// <copyright file="QueryExternalController.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Controllers.External
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Client;
    using Serilog;
    using Tim.Backend.Models.KustoQuery;
    using Tim.Backend.Providers.Database;
    using Tim.Backend.Providers.Kusto;
    using Tim.Backend.Providers.Query;

    /// <summary>
    /// QueryExternalController saves and executes kusto queries.
    /// </summary>
    [Route("api/query")]
    [ApiController]
    [Authorize] 
    public class QueryExternalController : Controller
    {
        private const string c_defaultKustoScope = "https://help.kusto.windows.net/.default";

        private readonly IConfidentialClientApplication m_authClient;
        private readonly IDatabaseRepository<KustoQueryRun> m_dbRepository;
        private readonly ILogger m_logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryExternalController"/> class.
        /// </summary>
        /// <param name="authClient">Auth client used to generate OBO token.</param>
        /// <param name="dbRepository">Database repository of operations.</param>
        public QueryExternalController(
            IConfidentialClientApplication authClient,
            IDatabaseRepository<KustoQueryRun> dbRepository)
        {
            m_dbRepository = dbRepository;
            m_authClient = authClient;
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
        public async Task<ActionResult<KustoQueryRun>> ExecuteQuery(
           [FromBody] KustoQuery queryRequest,
           CancellationToken cancellationToken)
        {
            queryRequest.Validate();

            var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");

            var activityName = "QueryExternalController-executeQuery";
            try
            {
                var kustoClient = await GetOBOKustoClient(queryRequest, accessToken);
                var delayQueryRunner = new DelayedQueryRunner(kustoClient, m_dbRepository);
                var queryRun = await delayQueryRunner.DelayRun(queryRequest, cancellationToken);

                return queryRun;
            }
            catch (Exception ex)
            {
                m_logger.Error(ex, "Failed to perform operation {operation} with exception: {exception}", activityName, ex);
                return Problem(title: "Query failed.", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
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
        public async Task<ActionResult<KustoQueryRun>> CheckQueryStatus(string queryRunId, CancellationToken cancellationToken)
        {
            var queryRun = await m_dbRepository.GetItemAsync(queryRunId);

            return queryRun == null ? NotFound() : Ok(queryRun);
        }

        private async Task<KustoQueryClient> GetOBOKustoClient(KustoQuery queryRequest, string accessToken)
        {
            var userAssertion = new UserAssertion(accessToken);
            var scopes = new string[] { c_defaultKustoScope };

            var requestToken = await m_authClient.AcquireTokenOnBehalfOf(scopes, userAssertion).ExecuteAsync();

            var kustoClient = KustoQueryClient.WithUserToken(requestToken.AccessToken, queryRequest.Cluster, queryRequest.Database);
            return kustoClient;
        }
    }
}
