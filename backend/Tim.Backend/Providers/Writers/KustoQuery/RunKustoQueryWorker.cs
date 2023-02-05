// <copyright file="RunKustoQueryWorker.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Writers.KustoQuery
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using Serilog;
    using StackExchange.Redis;
    using Tim.Backend.DataProviders.Clients;
    using Tim.Backend.Models;
    using Tim.Backend.Models.Events;
    using Tim.Backend.Models.KustoQuery;
    using Tim.Backend.Providers.Database;
    using Tim.Backend.Providers.Helpers;
    using Tim.Backend.Providers.Readers;

    /// <summary>
    /// Run Kusto query worker class to help organize everything for running query on behalf of user.
    /// </summary>
    public class RunKustoQueryWorker : IKustoQueryWorker
    {
        private readonly IConnectionMultiplexer m_redisConnection;
        private readonly KustoIngestClient m_ingestClient;
        private readonly ILogger m_logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RunKustoQueryWorker"/> class.
        /// </summary>
        /// <param name="redisConnection">Redis connectin string.</param>
        /// <param name="kustoIngestClient">Kusto ingest client.</param>
        public RunKustoQueryWorker(IConnectionMultiplexer redisConnection, KustoIngestClient kustoIngestClient)
        {
            m_redisConnection = redisConnection;
            m_ingestClient = kustoIngestClient;
            m_logger = Log.Logger;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RunKustoQueryWorker"/> class.
        /// </summary>
        public RunKustoQueryWorker()
        {
        }

        /// <summary>
        /// Run query gets everything ready to executed query.
        /// </summary>
        /// <param name="token">User token.</param>
        /// <param name="data">Query to be executed specifics.</param>
        /// <param name="customReader">Class that hosts execute query.</param>
        /// <param name="dbRepository">BucketName client.</param>
        /// <param name="kustoClient">Kusto client for run query.</param>
        /// <param name="kustoTableName">Kusto table name to write results.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Always returns true if instace runs to completion.</returns>
        public async Task<bool> RunQuery(string token, KustoQueryEventToProcess data, IKustoUserReader customReader, IDatabaseRepository<KustoQueryRun> dbRepository, KustoQueryClient kustoClient, string kustoTableName, CancellationToken cancellationToken)
        {
            var queryRunRecord = await dbRepository.GetItemAsync(data.QueryRunId.ToString());

            var strategicTrapHits = Enumerable.Empty<StrategicTrapHit>();
            var strategicTrapHitsEvents = Enumerable.Empty<IDictionary<string, object>>();

            // just in case the query record is null, sleep and try to get it again
            if (queryRunRecord == null)
            {
                Thread.Sleep(1000);
                queryRunRecord = await dbRepository.GetItemAsync(data.QueryRunId.ToString());
                if (queryRunRecord == null)
                {
                    m_logger.Warning(new NullReferenceException("Query run is null"), $"Exception failure due to query Run being null, query run id {data.QueryRunId}", "RunKustoQueryWorker-RunQuery");
                    return true;
                }
            }

            try
            {
                var strategicTrapHitsQueryDetails = await customReader.ExecuteQuery(kustoClient, token, data.Cluster, data.Database, data.Query, data.QueryRunId.ToString(), data.QueryId.ToString(), data.StartTime, data.EndTime, cancellationToken);

                strategicTrapHitsEvents = strategicTrapHitsQueryDetails.QueryResults;

                strategicTrapHits = strategicTrapHitsEvents.Select(hit => new StrategicTrapHit()
                {
                    EventAsJson = hit,
                    EntityId = Guid.NewGuid(),
                    QueryId = data.QueryId,
                    QueryRunId = data.QueryRunId,
                    QueryExecutionDatetTimeUtc = DateTime.UtcNow,
                }).ToList();

                queryRunRecord.ResultRowCount = strategicTrapHitsEvents.Count();
                m_logger.Warning($"Query is done executing, produced {queryRunRecord.ResultRowCount} results.", "RunKustoQueryWorker-RunQuery");
                queryRunRecord.ExecutionMetrics = strategicTrapHitsQueryDetails.Stats;
            }
            catch (Kusto.Data.Exceptions.KustoBadRequestException kustoException)
            {
                m_logger.Error(kustoException, $"Kusto failure {kustoException.Message}", "RunKustoQueryWorker-RunQuery");
                queryRunRecord.Errors = kustoException;
                queryRunRecord.Status = QueryRunStates.Error;
                queryRunRecord.MainError = "Exception during query execution: " + kustoException.Message;
                var queryRunError = await dbRepository.AddOrUpdateItemAsync(queryRunRecord.QueryRunId.ToString(), queryRunRecord);
                await m_ingestClient.WriteAsync(new List<KustoQueryRun>() { queryRunError }, kustoTableName, cancellationToken);
                return true;
            }
            catch (Exception e)
            {
                m_logger.Error(e, $"Exception failure {e.Message}", "RunKustoQueryWorker-RunQuery");
                queryRunRecord.Errors = e;
                queryRunRecord.Status = QueryRunStates.Error;
                queryRunRecord.MainError = "Exception during query execution: " + e.Message;
                var queryRunError = await dbRepository.AddOrUpdateItemAsync(queryRunRecord.QueryRunId.ToString(), queryRunRecord);
                await m_ingestClient.WriteAsync(new List<KustoQueryRun>() { queryRunError }, kustoTableName, cancellationToken);
                return true;
            }

            try
            {
                m_logger.Information("About to save query results to Redis.", "RunKustoQueryWorker-RunQuery");

                // TODO: figure out if result should be logged
                var result = await m_redisConnection
                    .GetDatabase()
                    .StringSetAsync(
                        RedisKeyGenerator.QueryRun(data.QueryRunId),
                        JsonConvert.SerializeObject(strategicTrapHitsEvents),
                        TimeSpan.FromMinutes(10));
            }
            catch (Exception e)
            {
                m_logger.Error(e, $"Exception failure when writing to blob {e.Message}. ", "RunKustoQueryWorker-RunQuery");
                queryRunRecord.Errors = e;
                queryRunRecord.Status = QueryRunStates.Error;
                queryRunRecord.MainError = "Exception while saving results: " + e.Message;
                var queryRunError = await dbRepository.AddOrUpdateItemAsync(queryRunRecord.QueryRunId.ToString(), queryRunRecord);
                await m_ingestClient.WriteAsync(new List<KustoQueryRun>() { queryRunError }, kustoTableName, cancellationToken);
            }

            // once results are written update query run cosmos db table and write record of the query run to kusto
            queryRunRecord.Status = QueryRunStates.Completed;
            var queryRun = await dbRepository.AddOrUpdateItemAsync(queryRunRecord.QueryRunId.ToString(), queryRunRecord);
            await m_ingestClient.WriteAsync(new List<KustoQueryRun>() { queryRun }, kustoTableName, cancellationToken);

            return true;
        }
    }
}
