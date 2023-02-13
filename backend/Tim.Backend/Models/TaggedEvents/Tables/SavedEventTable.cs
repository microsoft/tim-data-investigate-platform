// <copyright file="SavedEventTable.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.TaggedEvents.Tables
{
    using System;
    using System.Collections.Generic;
    using Kusto.Data.Common;

    /// <summary>
    /// Kusto table mappings for saved event.
    /// </summary>
    public class SavedEventTable : IKustoTable
    {
        /// <inheritdoc/>
        public List<ColumnMapping> ColumnMappings { get; } = new List<ColumnMapping>
        {
            new ColumnMapping { ColumnName = "EventId", ColumnType = "string", Properties = new Dictionary<string, string> { { MappingConsts.Path, "$.eventId" } } },
            new ColumnMapping { ColumnName = "EventTime", ColumnType = "datetime", Properties = new Dictionary<string, string> { { MappingConsts.Path, "$.eventTime" } } },
            new ColumnMapping { ColumnName = "DateTimeUtc", ColumnType = "datetime", Properties = new Dictionary<string, string> { { MappingConsts.Path, "$.dateTimeUtc" } } },
            new ColumnMapping { ColumnName = "CreatedBy", ColumnType = "string", Properties = new Dictionary<string, string> { { MappingConsts.Path, "$.createdBy" } } },
            new ColumnMapping { ColumnName = "EventAsJson", ColumnType = "dynamic", Properties = new Dictionary<string, string> { { MappingConsts.Path, "$.eventAsJson" } } },
        };

        /// <inheritdoc/>
        public string TableMappingName { get; } = "SavedEventMapping";

        /// <inheritdoc/>
        public string TableName { get; } = "SavedEvent";

        /// <inheritdoc/>
        public IEnumerable<Tuple<string, string>> TableSchema { get; } = new[]
        {
            Tuple.Create("EventId", "System.String"),
            Tuple.Create("EventTime", "System.DateTime"),
            Tuple.Create("DateTimeUtc", "System.DateTime"),
            Tuple.Create("CreatedBy", "System.String"),
            Tuple.Create("EventAsJson", "System.Object"),
        };

        /// <inheritdoc/>
        public string DatabaseName { get; set; }
    }
}
