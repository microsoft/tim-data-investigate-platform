// <copyright file="EventTagTable.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.TaggedEvents.Tables
{
    using System;
    using System.Collections.Generic;
    using Kusto.Data.Common;

    /// <summary>
    /// Kusto table mappings for event tags.
    /// </summary>
    public class EventTagTable : IKustoTable
    {
        /// <inheritdoc/>
        public List<ColumnMapping> ColumnMappings { get; } = new List<ColumnMapping>
        {
            new ColumnMapping { ColumnName = "EventId", ColumnType = "string", Properties = new Dictionary<string, string> { { MappingConsts.Path, "$.eventId" } } },
            new ColumnMapping { ColumnName = "DateTimeUtc", ColumnType = "datetime", Properties = new Dictionary<string, string> { { MappingConsts.Path, "$.dateTimeUtc" } } },
            new ColumnMapping { ColumnName = "CreatedBy", ColumnType = "string", Properties = new Dictionary<string, string> { { MappingConsts.Path, "$.createdBy" } } },
            new ColumnMapping { ColumnName = "Tag", ColumnType = "string", Properties = new Dictionary<string, string> { { MappingConsts.Path, "$.tag" } } },
            new ColumnMapping { ColumnName = "IsDeleted", ColumnType = "bool", Properties = new Dictionary<string, string> { { MappingConsts.Path, "$.isDeleted" } } },
        };

        /// <inheritdoc/>
        public string TableMappingName { get; } = "EventTagMapping";

        /// <inheritdoc/>
        public string TableName { get; } = "EventTag";

        /// <inheritdoc/>
        public IEnumerable<Tuple<string, string>> TableSchema { get; } = new[]
        {
            Tuple.Create("EventId", "System.String"),
            Tuple.Create("DateTimeUtc", "System.DateTime"),
            Tuple.Create("CreatedBy", "System.String"),
            Tuple.Create("Tag", "System.String"),
            Tuple.Create("IsDeleted", "System.Boolean"),
        };

        /// <inheritdoc/>
        public string DatabaseName { get; set; }
    }
}
