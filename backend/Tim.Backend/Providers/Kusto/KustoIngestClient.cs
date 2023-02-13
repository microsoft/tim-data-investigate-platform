// <copyright file="KustoIngestClient.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Kusto
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Kusto.Data.Common;
    using global::Kusto.Ingest;
    using Newtonsoft.Json;
    using Serilog;
    using Tim.Backend.Models.TaggedEvents;
    using Tim.Backend.Models.TaggedEvents.Tables;

    /// <summary>
    /// Kusto ingest client, responsible to ingesting data into kusto.
    /// </summary>
    public class KustoIngestClient
    {
        private readonly IKustoIngestClient m_client;
        private readonly ILogger m_logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="KustoIngestClient"/> class.
        /// </summary>
        /// <param name="client">Kusto client.</param>
        public KustoIngestClient(IKustoIngestClient client)
        {
            m_logger = Log.Logger;
            m_client = client;
        }

        /// <summary>
        /// Writes data into kusto.
        /// </summary>
        /// <param name="rows">Rows of data to be written.</param>
        /// <param name="kustoTable">Table specification.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Status of request.</returns>
        public async Task WriteAsync(IEnumerable<IKustoEvent> rows, IKustoTable kustoTable, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var ingestionProperties = new KustoIngestionProperties(kustoTable.DatabaseName, kustoTable.TableName)
            {
                Format = DataSourceFormat.json,
                IngestionMapping = new IngestionMapping()
                {
                    IngestionMappingReference = kustoTable.TableMappingName,
                },
            };

            m_logger.Information($"About to trigger ingestion via KustoIngestClient for table {kustoTable.TableName}.", "KustoIngestClient-WriteAsync");

            // TODO figure out if the results should be logged somehow so it can be recorded if it fails/how often this operation is done/etc
            var stream = await CreateStreamKustoEvents(rows);
            var result = await m_client.IngestFromStreamAsync(
                stream,
                ingestionProperties);
            await stream.DisposeAsync();

            m_logger.Information($"Ingestion completed with {result.GetIngestionStatusCollection().FirstOrDefault().Status}", "KustoIngestClient-WriteAsync");
        }

        private static async Task<Stream> CreateStreamKustoEvents(IEnumerable<IKustoEvent> rows)
        {
            var stream = new MemoryStream();
            using var writer = new StreamWriter(stream, Encoding.UTF8, 4096, true);

            foreach (var row in rows)
            {
                await writer.WriteLineAsync(JsonConvert.SerializeObject(row));
            }

            await writer.FlushAsync();

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
