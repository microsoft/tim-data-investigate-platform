// <copyright file="KustoQueryClient.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Kusto
{
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
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
    /// Kusto query client, responsible for querying kusto.
    /// </summary>
    public class KustoQueryClient : IKustoQueryClient
    {
        private readonly ICslQueryProvider m_client;
        private readonly ICslAdminProvider m_commandClient;
        private readonly ILogger m_logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="KustoQueryClient"/> class.
        /// </summary>
        public KustoQueryClient()
        {
            m_logger = Log.Logger;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KustoQueryClient"/> class.
        /// </summary>
        /// <param name="client">Kusto client.</param>
        /// <param name="commandClient">Kusto command client.</param>
        public KustoQueryClient(ICslQueryProvider client, ICslAdminProvider commandClient = null)
            : this()
        {
            m_client = client;
            m_commandClient = commandClient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KustoQueryClient"/> class.
        /// </summary>
        /// <param name="builder">Kusto string builder.</param>
        public KustoQueryClient(KustoConnectionStringBuilder builder)
            : this()
        {
            m_client = KustoClientFactory.CreateCslQueryProvider(builder);
            m_commandClient = KustoClientFactory.CreateCslAdminProvider(builder);
        }

        /// <summary>
        /// Retrieve the cluster schema.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<object> ShowClusterSchemaAsync()
        {
            m_logger.Information("Executing show schema command.");
            var result = await m_commandClient.ExecuteControlCommandAsync(
                m_commandClient.DefaultDatabaseName,
                ".show schema as json");
            return CreateResultsFromTable(result);
        }

        /// <inheritdoc/>
        public async Task<KustoQueryResults<IDictionary<string, object>>> RunQuery(KustoQuery query, CancellationToken cancellationToken)
        {
            m_logger.Information($"Executing kusto query on cluster {query.Cluster}.", "KustoQueryClient-RunQuery");
            var properties = new ClientRequestProperties();

            if (query.StartTime != null)
            {
                properties.SetParameter("StartTime", query.StartTime.Value.ToString("o", CultureInfo.InvariantCulture));
            }

            if (query.EndTime != null)
            {
                properties.SetParameter("EndTime", query.EndTime.Value.ToString("o", CultureInfo.InvariantCulture));
            }

            var dataSet = await m_client.ExecuteQueryV2Async(query.Database, query.QueryText, properties);
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
                    var frameResults = HandleDataTable(frame as ProgressiveDataSetDataTableFrame);
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

        private static KustoQueryResults<IDictionary<string, object>> HandleDataTable(ProgressiveDataSetDataTableFrame frame)
        {
            var queryResults = Enumerable.Empty<IDictionary<string, object>>();
            KustoQueryStats queryStats = null;

            using var table = frame.TableData;
            do
            {
                if (frame.TableKind == WellKnownDataSet.PrimaryResult)
                {
                    queryResults = CreateResultsFromTable(table);
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

        private static IEnumerable<IDictionary<string, object>> CreateResultsFromTable(IDataReader table)
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

        private static KustoQueryStats GetQueryStats(IDataReader table)
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
