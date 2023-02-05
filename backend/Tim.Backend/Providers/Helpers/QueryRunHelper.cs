// <copyright file="QueryRunHelper.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Tim.Backend.DataProviders.Clients;
    using Tim.Backend.Models;
    using Tim.Backend.Models.KustoQuery;
    using Tim.Backend.Models.KustoQuery.Api;
    using Tim.Backend.Providers.Database;

    /// <summary>
    /// Query run helper for on behalf of user queries.
    /// </summary>
    public class QueryRunHelper
    {
        /// <summary>
        /// Populates all query specific metadata for non saved queries.
        /// </summary>
        /// <param name="queryRequest">Request that came into the api.</param>
        /// <param name="dbRepository">BucketName client.</param>
        /// <param name="ingestClient">Kusto Ingest client.</param>
        /// <param name="containerName">Name of the kusto query run collection.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Kusto query event to process that gets pass on to complete the request.</returns>
        public static async Task<KustoQueryEventToProcess> ExecuteQueryHelper(KustoQueryExecuteRequest queryRequest, IDatabaseRepository<KustoQueryRun> dbRepository, KustoIngestClient ingestClient, string containerName, CancellationToken cancellationToken)
        {
            var queryRunRecord = new KustoQueryRun(queryRequest.RequestedBy, Guid.Empty, queryRequest.StartTime, queryRequest.EndTime);

            // record the run even if there were no results
            var queryRunRecordSaved = await dbRepository.AddOrUpdateItemAsync(queryRunRecord.QueryRunId.ToString(), queryRunRecord);
            await ingestClient.WriteAsync(new List<KustoQueryRun>() { queryRunRecordSaved }, containerName, cancellationToken);

            // pre make a query execution thing
            var queryToRun = new KustoQueryEventToProcess
            {
                QueryRunId = queryRunRecordSaved.QueryRunId,
                QueryId = Guid.Empty,
                Query = queryRequest.Query,
                StartTime = queryRunRecordSaved.QueryStartDateTimeUtc,
                EndTime = queryRunRecordSaved.QueryEndDateTimeUtc,
                Cluster = queryRequest.Cluster,
                Database = queryRequest.Database,
            };

            return queryToRun;
        }
    }
}
