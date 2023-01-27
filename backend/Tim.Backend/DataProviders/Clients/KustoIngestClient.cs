// <copyright file="KustoIngestClient.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.DataProviders.Clients
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kusto.Data;
    using Kusto.Data.Common;
    using Kusto.Ingest;
    using Newtonsoft.Json;
    using Serilog;

    /// <summary>
    /// Kusto Ingest Client, responsible to ingesting data into kusto.
    /// </summary>
    public class KustoIngestClient
    {
        private readonly IKustoQueuedIngestClient m_client;
        private readonly string m_kustoDatabase;
        private readonly JsonSerializer m_serializer = new();
        private readonly ILogger m_logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="KustoIngestClient"/> class.
        /// </summary>
        /// <param name="connectionStringBuilder">Connection sting builder.</param>
        public KustoIngestClient(KustoConnectionStringBuilder connectionStringBuilder)
        {
            m_logger = Log.Logger;
            m_logger.Information($"Initializing KustoIngestClient for cluster {connectionStringBuilder.DataSource} database {connectionStringBuilder.InitialCatalog}.");
            m_client = KustoIngestFactory.CreateQueuedIngestClient(connectionStringBuilder);
            m_kustoDatabase = connectionStringBuilder.InitialCatalog;
            m_logger.Information("Done initializing KustoIngestClient.");
        }

        /// <summary>
        /// Writes data into kusto.
        /// </summary>
        /// <typeparam name="T">Object type to be written.</typeparam>
        /// <param name="rows">Rows of data to be written.</param>
        /// <param name="tableName">Which table the data will be written to.</param>
        /// <param name="cancel">Cancellation token.</param>
        /// <returns>Status of request.</returns>
        public async Task WriteAsync<T>(IEnumerable<T> rows, string tableName, CancellationToken cancel)
        {
            cancel.ThrowIfCancellationRequested();

            var ingestionProperties = new KustoIngestionProperties(m_kustoDatabase, tableName)
            {
                Format = DataSourceFormat.multijson,
            };

            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            using var jsonWriter = new JsonTextWriter(writer);
            m_serializer.Serialize(jsonWriter, rows);
            await jsonWriter.FlushAsync(cancel);
            stream.Position = 0;

            m_logger.Information($"About to trigger ingestion via KustoIngestClient for table {tableName} rowcount {rows.Count()}.", "KustoIngestClient-WriteAsync");

            // TODO figure out if the results should be logged somehow so it can be recorded if it fails/how often this operation is done/etc
            var result = await m_client.IngestFromStreamAsync(stream, ingestionProperties);

            m_logger.Information($"Ingestion completed with {result.GetIngestionStatusCollection().FirstOrDefault().Status}", "KustoIngestClient-WriteAsync");
        }
    }
}
