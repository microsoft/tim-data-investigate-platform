// <copyright file="KustoAdminClient.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Kusto
{
    using System.Threading.Tasks;
    using global::Kusto.Data.Common;
    using global::Kusto.Data.Ingestion;
    using Serilog;
    using Tim.Backend.Models.TaggedEvents.Tables;

    /// <summary>
    /// Kusto admin client, responsible for performing control commands.
    /// </summary>
    public class KustoAdminClient
    {
        private readonly ICslAdminProvider m_client;
        private readonly ILogger m_logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="KustoAdminClient"/> class.
        /// </summary>
        /// <param name="client">Kusto client.</param>
        public KustoAdminClient(ICslAdminProvider client)
        {
            m_logger = Log.Logger;
            m_client = client;
        }

        /// <summary>
        /// Create a new table.
        /// </summary>
        /// <param name="kustoTable">Specifications for the Kusto table to create.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task CreateTableAsync(IKustoTable kustoTable)
        {
            var command = CslCommandGenerator.GenerateTableCreateCommand(kustoTable.TableName, kustoTable.TableSchema);
            m_logger.Information($"Attempting to create new table {kustoTable.TableName}.", "KustoAdminClient-CreateTableAsync");
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
            m_logger.Information($"Attempting to create new table mapping {kustoTable.TableMappingName}.", "KustoAdminClient-CreateTableMappingAsync");
            await m_client.ExecuteControlCommandAsync(kustoTable.DatabaseName, command);
        }
    }
}
