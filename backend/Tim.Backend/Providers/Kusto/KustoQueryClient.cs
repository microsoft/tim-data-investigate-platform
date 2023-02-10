// <copyright file="KustoQueryClient.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Kusto
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using global::Kusto.Data;
    using global::Kusto.Data.Common;
    using global::Kusto.Data.Net.Client;
    using global::Kusto.Data.Results;
    using Newtonsoft.Json;
    using Serilog;
    using Tim.Backend.Models.KustoQuery;
    using Tim.Backend.Providers.Query;

    /// <summary>
    /// Kusto Query client is used to query kusto tables.
    /// </summary>
    public class KustoQueryClient : IKustoQueryClient
    {
        private readonly IKustoStatelessClient m_client;
        private readonly ILogger m_logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="KustoQueryClient"/> class.
        /// </summary>
        /// <param name="connectionStringBuilder">Connection string builder.</param>
        public KustoQueryClient(KustoConnectionStringBuilder connectionStringBuilder)
        {
            m_logger = Log.Logger;
            var type = !string.IsNullOrEmpty(connectionStringBuilder.UserToken) ? "user" : "service";
            m_logger.Information($"Initializing KustoQueryClient for cluster: {connectionStringBuilder.DataSource} database: {connectionStringBuilder.InitialCatalog} Client type: {type}.");
            m_client = KustoClientFactory.CreateCslQueryProvider(connectionStringBuilder) as IKustoStatelessClient;
            m_logger.Information("Done initializing KustoQueryClient.");
        }

        /// <summary>
        /// Create kusto client using user token.
        /// </summary>
        /// <param name="token">User bearer token.</param>
        /// <param name="clusterUrl">Cluster url the query will be run on top of.</param>
        /// <param name="databaseName">Database the query will be run on top of.</param>
        /// <returns>Kusto Query Client ready to execute query.</returns>
        public static KustoQueryClient WithUserToken(string token, string clusterUrl, string databaseName)
        {
            var builder = new KustoConnectionStringBuilder(clusterUrl, databaseName)
                .WithAadUserTokenAuthentication(token);
            return new KustoQueryClient(builder);
        }

        /// <summary>
        /// Execute query and get results.
        /// </summary>
        /// <typeparam name="T">Object that results will be parsed into.</typeparam>
        /// <param name="query">Query to be executes.</param>
        /// <param name="properties">Properties to run the query with, if not provided, use defaults.</param>
        /// <returns>Query Results.</returns>
        /// <exception cref="UnexpectedFrameException">Formatting error.</exception>
        public async Task<KustoQueryResults<T>> ReadAsync<T>(KustoQuery query, ClientRequestProperties properties = null)
            where T : class, new()
        {
            var queryResults = new List<T>();
            KustoQueryStats queryStats = null;

            using var frames = await ExecuteQueryAsync(query, properties);

            while (frames.MoveNext())
            {
                var frame = frames.Current;

                if (frame.FrameType == FrameType.DataSetHeader)
                {
                    continue;
                }
                else if (frame.FrameType == FrameType.DataTable)
                {
                    var frameResults = HandleDataTable<T>(frame as ProgressiveDataSetDataTableFrame);
                    queryResults.AddRange(frameResults.QueryResults);
                    if (frameResults.QueryStats != null)
                    {
                        queryStats = frameResults.QueryStats;
                    }
                }
                else if (frame.FrameType == FrameType.DataSetCompletion)
                {
                    continue;
                }
                else
                {
                    var ex = new UnexpectedFrameException($"Received unexpected frame type `{frame.FrameType}`.");
                    m_logger.Error(ex, "Failed to perform operation {operation} with exception: {exception}", "KustoQueryClient-ReadAsync", ex);
                    throw ex;
                }
            }

            return new KustoQueryResults<T>(queryResults, queryStats);
        }

        /// <inheritdoc/>
        public async Task<KustoQueryResults<IDictionary<string, object>>> RunQuery(KustoQuery query, CancellationToken cancellationToken)
        {
            var dataSet = await m_client.ExecuteQueryV2Async(query.Cluster, query.Database, query.Query, null, cancellationToken);
            var queryResults = new List<IDictionary<string, object>>();
            KustoQueryStats queryStats = null;
            var frames = dataSet.GetFrames();

            while (frames.MoveNext())
            {
                var frame = frames.Current;

                if (frame.FrameType == FrameType.DataSetHeader)
                {
                    continue;
                }
                else if (frame.FrameType == FrameType.DataTable)
                {
                    var frameResults = HandleDataTableUntyped(frame as ProgressiveDataSetDataTableFrame);
                    queryResults.AddRange(frameResults.QueryResults);
                    if (frameResults.QueryStats != null)
                    {
                        queryStats = frameResults.QueryStats;
                    }
                }
                else if (frame.FrameType == FrameType.DataSetCompletion)
                {
                    continue;
                }
                else
                {
                    var ex = new UnexpectedFrameException($"Received unexpected frame type `{frame.FrameType}`.");
                    m_logger.Error(ex, "Failed to perform operation {operation} with exception: {exception}", "KustoQueryClient-RunQuery", ex);
                    throw ex;
                }
            }

            return new KustoQueryResults<IDictionary<string, object>>(queryResults, queryStats);
        }

        private async Task<IEnumerator<ProgressiveDataSetFrame>> ExecuteQueryAsync(KustoQuery query, ClientRequestProperties properties = null)
        {
            ClientRequestProperties clientRequestProperties;
            if (properties == null)
            {
                clientRequestProperties = new ClientRequestProperties()
                {
                    // Provides breadcrumb in Kusto performance logs
                    ClientRequestId = $"TimBackendQuery;{Guid.NewGuid()}",
                };
                clientRequestProperties.SafeSetQueryResultsProgressiveEnabled(false);
            }
            else
            {
                clientRequestProperties = properties;
            }

            m_logger.Information($"Executing query using Kusto's ExecuteQueryV2Async. Client Request Id: {clientRequestProperties.ClientRequestId} ", "KustoQueryClient-ExecuteQueryAsync");
            var dataSet = await m_client.ExecuteQueryV2Async(query.Cluster, query.Database, query.Query, clientRequestProperties);
            m_logger.Information($"Query completed. Client Request Id: {clientRequestProperties.ClientRequestId} ", "KustoQueryClient-ExecuteQueryAsync");
            return dataSet.GetFrames();
        }

        /// <summary>
        /// Parse query results.
        /// </summary>
        /// <typeparam name="T">object type that results will be parsed into.</typeparam>
        /// <param name="frame">Data table frame returned by kusto.</param>
        /// <returns>Query Results.</returns>
        private KustoQueryResults<T> HandleDataTable<T>(ProgressiveDataSetDataTableFrame frame)
            where T : class, new()
        {
            var queryResults = Enumerable.Empty<T>();
            KustoQueryStats queryStats = null;

            using var table = frame.TableData;
            do
            {
                if (frame.TableKind == WellKnownDataSet.PrimaryResult)
                {
                    queryResults = CreateObjectsFromTable<T>(table);
                }
                else if (frame.TableKind == WellKnownDataSet.QueryCompletionInformation)
                {
                    queryStats = GetQueryStats(table);
                }
                else
                {
                    while (table.Read())
                    {
                        // We need to materialize the results to continue to the next frame, but we don't care about the data.
                        // This is enforced by the Kusto.Data client, which throws an exception if we don't, though to be honest I'm not quite sure why.
                    }
                }
            }
            while (table.NextResult());

            return new KustoQueryResults<T>(queryResults, queryStats);
        }

        private KustoQueryResults<IDictionary<string, object>> HandleDataTableUntyped(ProgressiveDataSetDataTableFrame frame)
        {
            var queryResults = Enumerable.Empty<IDictionary<string, object>>();
            KustoQueryStats queryStats = null;

            using var table = frame.TableData;
            do
            {
                if (frame.TableKind == WellKnownDataSet.PrimaryResult)
                {
                    queryResults = CreateUntypedResultsFromTable(table);
                }
                else if (frame.TableKind == WellKnownDataSet.QueryCompletionInformation)
                {
                    queryStats = GetQueryStats(table);
                }
                else
                {
                    while (table.Read())
                    {
                        // We need to materialize the results to continue to the next frame, but we don't care about the data.
                        // This is enforced by the Kusto.Data client, which throws an exception if we don't, though to be honest I'm not quite sure why.
                    }
                }
            }
            while (table.NextResult());

            return new KustoQueryResults<IDictionary<string, object>>(queryResults, queryStats);
        }

        private IEnumerable<T> CreateObjectsFromTable<T>(IDataReader table)
            where T : class, new()
        {
            var results = new List<T>();

            while (table.Read())
            {
                var result = new T();

                for (var i = 0; i < table.FieldCount; i++)
                {
                    var propertyName = table.GetName(i);
                    var property = result
                        .GetType()
                        .GetProperty(propertyName);

                    property?.SetValue(result, table.GetValue(i));
                }

                results.Add(result);
            }

            return results;
        }

        private IEnumerable<IDictionary<string, object>> CreateUntypedResultsFromTable(IDataReader table)
        {
            var results = new List<IDictionary<string, object>>();

            while (table.Read())
            {
                var result = new Dictionary<string, object>();

                for (var i = 0; i < table.FieldCount; i++)
                {
                    result.Add(table.GetName(i), table.GetValue(i));
                }

                results.Add(result);
            }

            return results;
        }

        private KustoQueryStats GetQueryStats(IDataReader table)
        {
            var levelNameColumnIndex = -1;
            var payloadColumnIndex = -1;

            for (var i = 0; i < table.FieldCount; i++)
            {
                if (table.GetName(i) == "LevelName")
                {
                    levelNameColumnIndex = i;
                }
                else if (table.GetName(i) == "Payload")
                {
                    payloadColumnIndex = i;
                }
            }

            while (table.Read())
            {
                if (table.GetValue(levelNameColumnIndex).ToString() == "Stats")
                {
                    return JsonConvert.DeserializeObject<KustoQueryStats>(table.GetValue(payloadColumnIndex).ToString());
                }
            }

            return null;
        }
    }
}
