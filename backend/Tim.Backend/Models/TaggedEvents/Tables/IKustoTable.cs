// <copyright file="IKustoTable.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.TaggedEvents.Tables
{
    using System;
    using System.Collections.Generic;
    using Kusto.Data.Common;

    /// <summary>
    /// Database interface.
    /// </summary>
    public interface IKustoTable
    {
        /// <summary>
        /// Gets the column mappings for kusto ingestion.
        /// </summary>
        public List<ColumnMapping> ColumnMappings { get; }

        /// <summary>
        /// Gets the kusto table mapping name.
        /// </summary>
        public string TableMappingName { get; }

        /// <summary>
        /// Gets the kusto table name.
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Gets or sets the kusto database name.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets a kusto table schema.
        /// </summary>
        public IEnumerable<Tuple<string, string>> TableSchema { get; }
    }
}
