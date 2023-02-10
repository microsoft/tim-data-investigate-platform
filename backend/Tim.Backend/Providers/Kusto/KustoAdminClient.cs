// <copyright file="KustoAdminClient.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Kusto
{
    using System.Threading.Tasks;
    using global::Kusto.Data;
    using global::Kusto.Data.Common;
    using global::Kusto.Data.Ingestion;
    using global::Kusto.Data.Net.Client;
    using Serilog;
    using Tim.Backend.Models.TaggedEvents.Tables;

    /// <summary>
    /// Kusto Ingest Client, responsible to ingesting data into kusto.
    /// </summary>
    public class KustoAdminClient
    {
        private readonly ICslAdminProvider m_client;
        private readonly ILogger m_logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="KustoAdminClient"/> class.
        /// </summary>
        /// <param name="connectionStringBuilder">Connection sting builder.</param>
        public KustoAdminClient(KustoConnectionStringBuilder connectionStringBuilder)
        {
            m_logger = Log.Logger;
            m_logger.Information($"Initializing KustoAdminClient for cluster {connectionStringBuilder.DataSource} database {connectionStringBuilder.InitialCatalog}.");
            m_client = KustoClientFactory.CreateCslAdminProvider(connectionStringBuilder);
            m_logger.Information("Done initializing KustoAdminClient.");
        }

        /// <summary>
        /// Create a new table.
        /// </summary>
        /// <param name="kustoTable">Specifications for the Kusto table to create.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task CreateTableAsync(IKustoTable kustoTable)
        {
            var command = CslCommandGenerator.GenerateTableCreateCommand(kustoTable.TableName, kustoTable.TableSchema);
            await m_client.ExecuteControlCommandAsync(kustoTable.DatabaseName, command);
        }

        /// <summary>
        /// Create the table ingest mappings.
        /// </summary>
        /// <param name="kustoTable">Specifications for the ingestion mappings.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task CreateTableMappingAsync(IKustoTable kustoTable)
        {
            var command = CslCommandGenerator.GenerateTableMappingCreateOrAlterCommand(
                IngestionMappingKind.Json,
                kustoTable.TableName,
                kustoTable.TableMappingName,
                kustoTable.ColumnMappings);
            await m_client.ExecuteControlCommandAsync(kustoTable.DatabaseName, command);
        }
    }
}
