// <copyright file="KustoUserReader.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Readers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kusto.Data.Common;
    using Serilog;
    using Tim.Backend.DataProviders.Clients;

    /// <summary>
    /// This class queries kusto on behalf of the user logged into the system.
    /// </summary>
    public class KustoUserReader : IKustoUserReader
    {
        private static readonly TimeSpan s_kustoTimeout = TimeSpan.FromMinutes(10);

        private readonly ILogger m_logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="KustoUserReader"/> class.
        /// </summary>
        public KustoUserReader()
        {
            m_logger = Log.Logger;
        }

        /// <summary>
        /// Execute query on behalf of a user.
        /// </summary>
        /// <param name="kustoClient">Kusto client that query kusto.</param>
        /// <param name="accessToken">User access token.</param>
        /// <param name="cluster">Kusto cluster where the query will be executed.</param>
        /// <param name="db">Databse where the query will be executed.</param>
        /// <param name="query">Query to be executed.</param>
        /// <param name="queryRunId">Unique id of this specific query execution instance.</param>
        /// <param name="queryId">If saved query is being executed, this would contain the saved query id.</param>
        /// <param name="startTime">Query window start time.</param>
        /// <param name="endTime">Query window end time.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Results of the executed query as a dictionary object.</returns>
        public async Task<KustoQueryResults<IDictionary<string, object>>> ExecuteQuery(KustoQueryClient kustoClient, string accessToken, string cluster, string db, string query, string queryRunId, string queryId, DateTime startTime, DateTime endTime, CancellationToken ct)
        {
            // 10/7/2022 - use app id/key for queries for now, will switch back to user based token later as part of refactoring so keeping it in the func for now
            var queryParameters = new Dictionary<string, string>()
            {
                { "StartTime", startTime.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) },
                { "EndTime", endTime.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) },
            };

            var clientRequestProperties = new ClientRequestProperties(
                options: new Dictionary<string, object>
                {
                    { ClientRequestProperties.OptionServerTimeout, s_kustoTimeout },
                },
                parameters: queryParameters)
            {
                // Provides breadcrumb in Kusto performance logs
                ClientRequestId = $"Tim.Backend.ExecuteCustomQuery;{Guid.NewGuid()}",
            };

            m_logger.Information($"Executing query on behalf of a user. ", "KustoUserReader-ExecuteQuery");
            var results = await kustoClient.ReadUntypedAsync(query, db, clientRequestProperties);

            // TODO replace with Promethus?
            /* m_logger.LogMetric("TriggerQuery", new Metric("TriggerCount", 1, new Dictionary<string, string>
                {
                    { "QueryRunId", queryId },
                    { "EventCount", results.QueryResults.Count().ToString() },
                }));
            */

            m_logger.Information($"Done executing query on behalf of a user, results returned {results.QueryResults.Count()}.", "KustoUserReader-ExecuteQuery");
            return results;
        }
    }
}
