// <copyright file="QueryExternalController.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Controllers.External
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.Identity.Web;
    using Newtonsoft.Json;
    using Serilog;
    using StackExchange.Redis;
    using Tim.Backend.DataProviders.Clients;
    using Tim.Backend.Models.KustoQuery;
    using Tim.Backend.Models.KustoQuery.Api;
    using Tim.Backend.Providers.Database;
    using Tim.Backend.Providers.Helpers;
    using Tim.Backend.Providers.Readers;
    using Tim.Backend.Providers.Writers.KustoQuery;
    using Tim.Backend.Startup.Config;
    using static Tim.Backend.Models.KustoQuery.KustoQueryRun;

    /// <summary>
    /// QueryExternalController saves and executes kusto queries.
    /// </summary>
    [Route("/external/query")]
    [ApiController]
    [Authorize]
    public class QueryExternalController : Controller
    {
        private readonly KustoIngestClient m_ingestClient;
        private readonly KustoQueryClient m_kustoQueryClient;
        private readonly IKustoUserReader m_userReader;
        private readonly IKustoQueryWorker m_kustoQueryExecutor;
        private readonly IOptions<DatabaseConfiguration> m_dbConfigs;
        private readonly IDatabaseClient m_dbClient;
        private readonly IConnectionMultiplexer m_redisConnection;
        private readonly ILogger m_logger;

        // TODO - bring this back when fixing OBO.
        // private readonly ITokenAcquisition m_tokenAcquisition;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryExternalController"/> class.
        /// </summary>
        /// <param name="userKustoReader">Kusto User reader used to execute kusto querires on behalf of user making the api call.</param>
        /// <param name="kustoClient">Kusto client that represents connection to Kusto used for ingestion.</param>
        /// <param name="kustoQueryClient">Kusto client that represents connection to Kusto used for executing queries.</param>
        /// <param name="tokenAcquisition">Process that gets OBO token for auth.</param>
        /// <param name="kustoQueryworker">Spins up OBO kusto flow.</param>
        /// <param name="configsDb">Represents appropriate configurations.</param>
        /// <param name="idbClient">Database client.</param>
        /// <param name="connectionMultiplexer">Redis location to temp store query results.</param>
        public QueryExternalController(
            IKustoUserReader userKustoReader,
            KustoIngestClient kustoClient,
            KustoQueryClient kustoQueryClient,
            ITokenAcquisition tokenAcquisition,
            IKustoQueryWorker kustoQueryworker,
            IOptions<DatabaseConfiguration> configsDb,
            IDatabaseClient idbClient,
            IConnectionMultiplexer connectionMultiplexer)
        {
            m_userReader = userKustoReader;
            m_ingestClient = kustoClient;
            m_kustoQueryExecutor = kustoQueryworker;
            m_dbClient = idbClient;
            m_dbConfigs = configsDb;
            m_redisConnection = connectionMultiplexer;
            m_kustoQueryClient = kustoQueryClient;
            m_logger = Log.Logger;
        }

        /// <summary>
        /// Execute a kusto query for a given time window.
        /// </summary>
        /// <param name="queryRequest">Details of the kusto query to be executed.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A query run quid to track status of query.</returns>
        [HttpPost("executeQuery")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<KustoQueryTriggerResponse>> ExecuteQuery(
           [FromBody] KustoQueryExecuteRequest queryRequest,
           CancellationToken cancellationToken)
        {
            queryRequest.Validate();

            var activityName = "QueryExternalController-executeQuery";
            try
            {
                if (string.IsNullOrWhiteSpace(queryRequest.Query))
                {
                    return BadRequest(ProblemDetailsFactory.CreateProblemDetails(
                        HttpContext,
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Kusto Query must be specified. "));
                }

                var queryRunRecord = await QueryRunHelper.ExecuteQueryHelper(queryRequest, m_dbClient, m_ingestClient, m_dbConfigs.Value.QueryRunsContainerName, cancellationToken);

                // TODO 2022-10-07 - switch to app id/key for kusto queries - will revert back to user executes queries once we have a chance to refactor auth
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable IDE0058 // Expression value is never used
                Task.Run(() => m_kustoQueryExecutor.RunQuery(string.Empty, queryRunRecord, m_userReader, m_dbClient, m_kustoQueryClient, m_dbConfigs.Value, CancellationToken.None));
#pragma warning restore IDE0058 // Expression value is never used
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                return new KustoQueryTriggerResponse(queryRunRecord.QueryRunId, "Query has been queued. ");
            }
            catch (Exception ex)
            {
                m_logger.Error(ex, "Failed to perform operation {operation} with exception: {exception}", activityName, ex);
                return Problem(title: $"Query failed.", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
        }

        /// <summary>
        /// Get the results of a kusto query that was requested to be executed.
        /// </summary>
        /// <param name="queryRunId">Query Run Id to pull status.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Query results when ready.</returns>
        [HttpGet("getQueryResult")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<KustoQueryRunActionResponse>> CheckQueryStatus(string queryRunId, CancellationToken cancellationToken)
        {
            var queryRun = await m_dbClient.GetItem<KustoQueryRun>(queryRunId, m_dbConfigs.Value.QueryRunsContainerName);

            if (queryRun == null)
            {
                return NotFound();
            }

            var activityName = "QueryExternalController-getQueryResult";

            if (queryRun.Status == QueryRunStates.Completed)
            {
                try
                {
                    // return the result
                    var response = new KustoQueryRunActionResponse(queryRun.QueryRunId, string.Empty);

                    if (queryRun.ResultRowCount != 0)
                    {
                        var resultString = await m_redisConnection
                            .GetDatabase()
                            .StringGetDeleteAsync(RedisKeyGenerator.QueryRun(queryRun.QueryRunId));
                        response.Results = JsonConvert.DeserializeObject(resultString);
                    }
                    else
                    {
                        response.Results = Array.Empty<int>();
                    }

                    response.RowCount = queryRun.ResultRowCount;
                    response.ExecutionMetrics = queryRun.ExecutionMetrics;
                    return response;
                }
                catch (Exception ex)
                {
                    m_logger.Error(ex, "Failed to perform operation {operation} with exception: {exception}", activityName, ex);
                    return Problem(title: $"Failure to get query results.", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
                }
            }
            else if (queryRun.Status == QueryRunStates.Error)
            {
                return Problem(title: $"Query failed.", detail: queryRun.MainError, statusCode: StatusCodes.Status400BadRequest);
            }
            else if (DateTime.UtcNow - queryRun.TriggeredDateTimeUtc > TimeSpan.FromMinutes(30) || queryRun.Status == QueryRunStates.TimedOut)
            {
                if (queryRun.Status != QueryRunStates.TimedOut)
                {
                    queryRun.Status = QueryRunStates.TimedOut;
#pragma warning disable IDE0058 // Expression value is never used
                    await m_dbClient.AddOrUpdateItem<KustoQueryRun>(queryRun.QueryRunId.ToString(), JsonConvert.SerializeObject(queryRun), m_dbConfigs.Value.QueryRunsContainerName);
#pragma warning restore IDE0058 // Expression value is never used
                }

                return Problem(title: $"Query timed out.", detail: "Query sent no response and timeout of 10 minutes was reached. ", statusCode: StatusCodes.Status400BadRequest);
            }
            else
            {
                return Accepted();
            }
        }
    }
}
