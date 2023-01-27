// <copyright file="KustoQueryEventToProcess.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models
{
    using System;

    /// <summary>
    /// Represents an instance of a query that is about to be run.
    /// </summary>
    public class KustoQueryEventToProcess
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KustoQueryEventToProcess"/> class.
        /// </summary>
        public KustoQueryEventToProcess()
        {
        }

        /// <summary>
        /// Gets or sets query run Id.
        /// </summary>
        public Guid QueryRunId { get; set; }

        /// <summary>
        /// Gets or sets query Id.
        /// </summary>
        public Guid QueryId { get; set; }

        /// <summary>
        /// Gets or sets the kusto query.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets the kusto cluster where the query will be run.
        /// </summary>
        public string Cluster { get; set; }

        /// <summary>
        /// Gets or sets the kusto db where thequery will be run.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the Start time for the kusto query time window for current query execution.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the End time for the kusto query time window for current query execution.
        /// </summary>
        public DateTime EndTime { get; set; }
    }
}