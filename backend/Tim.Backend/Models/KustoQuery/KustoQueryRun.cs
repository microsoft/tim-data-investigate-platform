// <copyright file="KustoQueryRun.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.KustoQuery
{
    using System;
    using Tim.Backend.DataProviders.Clients;

    /// <summary>
    /// This object represents an instance of an executed query
    /// Query can be a saved one or simply a temporary one that was passed over via a request
    /// In tha same the query is temporary, the Query GUid will be an empty Guid
    /// otherwise the query guid will represent the Guid that can be used to find the actual query in the SavedQueries table in cosmosdb.
    /// </summary>
    public class KustoQueryRun
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KustoQueryRun"/> class.
        /// </summary>
        /// <param name="triggeredBy">Who the query run was triggered by.</param>
        /// <param name="queryId">Query to be run.</param>
        /// <param name="start">Start time window of query to be executed.</param>
        /// <param name="end">End time window of query to be executed.</param>
        public KustoQueryRun(string triggeredBy, Guid queryId, DateTime start, DateTime end)
        {
            TriggeredBy = triggeredBy;
            QueryId = queryId;
            QueryRunId = Guid.NewGuid();
            QueryStartDateTimeUtc = start;
            QueryEndDateTimeUtc = end;
            TriggeredDateTimeUtc = DateTime.UtcNow;
            Status = QueryRunStates.Created;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KustoQueryRun"/> class.
        /// </summary>
        public KustoQueryRun()
        {
        }

        /// <summary>
        /// Query Run states.
        /// </summary>
        public enum QueryRunStates
        {
#pragma warning disable SA1602 // Enumeration items should be documented
            Created,
            Completed,
            Error,
            TimedOut,
#pragma warning restore SA1602 // Enumeration items should be documented
        }

        /// <summary>
        /// Gets or sets who the query run was triggered by.
        /// </summary>
        public string TriggeredBy { get; set; }

        /// <summary>
        /// Gets or sets the time the query run was triggered.
        /// </summary>
        public DateTime TriggeredDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the query run Id.
        /// </summary>
        public Guid QueryRunId { get; set; }

        /// <summary>
        /// Gets or sets the query Id.
        /// </summary>
        public Guid QueryId { get; set; }

        /// <summary>
        /// Gets or sets the row count based on query run results.
        /// </summary>
        public int? ResultRowCount { get; set; }

        /// <summary>
        /// Gets or sets the execution metrics.
        /// </summary>
        public KustoQueryStats ExecutionMetrics { get; set; }

        /// <summary>
        /// Gets or sets the Errors if present.
        /// </summary>
        public Exception Errors { get; set; }

        /// <summary>
        /// Gets or sets the main error in case there are nested errors.
        /// </summary>
        public string MainError { get; set; }

        /// <summary>
        /// Gets or sets the query run start time.
        /// </summary>
        public DateTime QueryStartDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the query run end time.
        /// </summary>
        public DateTime QueryEndDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the query run status.
        /// </summary>
        public QueryRunStates Status { get; set; }
    }
}
