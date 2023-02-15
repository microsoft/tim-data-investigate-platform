// <copyright file="KustoExternalController.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Controllers.External
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Kusto.Data;
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
    /// KustoExternalController performs actions for Kusto.
    /// </summary>
    [Route("api/kusto")]
    [ApiController]
    [Authorize]
    public class KustoExternalController : Controller
    {
        private const string c_defaultKustoScope = "https://help.kusto.windows.net/.default";

        private readonly IConfidentialClientApplication m_authClient;
        private readonly IDatabaseRepository<KustoQueryRun> m_dbRepository;
        private readonly ILogger m_logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="KustoExternalController"/> class.
        /// </summary>
        /// <param name="authClient">Auth client used to generate OBO token.</param>
        /// <param name="dbRepository">Database repository of operations.</param>
        public KustoExternalController(
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
        /// <param name="clusterDatabase">Details of the kusto cluster.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A query run quid to track status of query.</returns>
        [HttpPost("schema")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetSchema(
           [FromBody] KustoClusterDatabase clusterDatabase,
           CancellationToken cancellationToken)
        {
            var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");

            var kustoClient = await GetKustoClientOnBehalfOf(clusterDatabase, accessToken);
            var schema = await kustoClient.ShowClusterSchemaAsync();
            return Ok(schema);
        }

        /// <summary>
        /// Execute a kusto query for a given time window.
        /// </summary>
        /// <param name="queryRequest">Details of the kusto query to be executed.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A query run quid to track status of query.</returns>
        [HttpPost("query")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<KustoQueryRun>> ExecuteQuery(
           [FromBody] KustoQuery queryRequest,
           CancellationToken cancellationToken)
        {
            var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");

            var kustoClient = await GetKustoClientOnBehalfOf(queryRequest, accessToken);
            var delayQueryRunner = new DelayedQueryRunner(kustoClient, m_dbRepository);
            var queryRun = await delayQueryRunner.DelayRun(queryRequest, cancellationToken);

            return queryRun.Status == QueryRunStatus.Created ? Accepted(queryRun) : Ok(queryRun);
        }

        /// <summary>
        /// Get the results of a kusto query that was requested to be executed.
        /// </summary>
        /// <param name="queryRunId">Query Run Id to pull status.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Query results when ready.</returns>
        [HttpGet("query/{queryRunId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<KustoQueryRun>> CheckQueryStatus(Guid queryRunId, CancellationToken cancellationToken)
        {
            var queryRun = await m_dbRepository.GetItemAsync(queryRunId.ToString());

            return queryRun == null ? NotFound() : queryRun.Status == QueryRunStatus.Created ? Accepted(queryRun) : Ok(queryRun);
        }

        private async Task<KustoQueryClient> GetKustoClientOnBehalfOf(KustoClusterDatabase clusterDatabase, string accessToken)
        {
            var userAssertion = new UserAssertion(accessToken);
            var scopes = new string[] { c_defaultKustoScope };

            m_logger.Information($"Requesting user token on behalf of.");
            var requestToken = await m_authClient.AcquireTokenOnBehalfOf(scopes, userAssertion).ExecuteAsync();

            var builder = new KustoConnectionStringBuilder(clusterDatabase.Cluster, clusterDatabase.Database)
                .WithAadUserTokenAuthentication(requestToken.AccessToken);

            return new KustoQueryClient(builder);
        }
    }
}
