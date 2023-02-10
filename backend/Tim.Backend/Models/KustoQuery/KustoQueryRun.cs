// <copyright file="KustoQueryRun.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.KustoQuery
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Query Run states.
    /// </summary>
    public enum QueryRunStates
    {
        /// <summary>
        /// Query first created.
        /// </summary>
        Created,

        /// <summary>
        /// Query completed.
        /// </summary>
        Completed,

        /// <summary>
        /// Query had an error.
        /// </summary>
        Error,

        /// <summary>
        /// Query timed out.
        /// </summary>
        TimedOut,
    }

    /// <summary>
    /// This object represents an instance of an executed query.
    /// </summary>
    public class KustoQueryRun : IJsonEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KustoQueryRun"/> class.
        /// </summary>
        /// <param name="query">Query details.</param>
        public KustoQueryRun(KustoQuery query)
        {
            KustoQuery = query;
            QueryRunId = Guid.NewGuid();
            ExecuteDateTimeUtc = DateTime.UtcNow;
            Status = QueryRunStates.Created;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KustoQueryRun"/> class.
        /// </summary>
        public KustoQueryRun()
        {
        }

        /// <inheritdoc/>
        public string Id => QueryRunId.ToString();

        /// <summary>
        /// Gets or sets the query associated with this query run.
        /// </summary>
        [JsonProperty("kustoQuery")]
        public KustoQuery KustoQuery { get; set; }

        /// <summary>
        /// Gets or sets the time the query run was triggered.
        /// </summary>
        [JsonProperty("executeDateTimeUtc")]
        public DateTime ExecuteDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the query run Id.
        /// </summary>
        [JsonProperty("queryRunId")]
        public Guid QueryRunId { get; set; }

        /// <summary>
        /// Gets or sets the query run status.
        /// </summary>
        [JsonProperty("status")]
        public QueryRunStates Status { get; set; }

        /// <summary>
        /// Gets or sets the resulting data from the query.
        /// </summary>
        [JsonProperty("resultData")]
        public IEnumerable<IDictionary<string, object>> ResultData { get; set; }

        /// <summary>
        /// Gets or sets the execution metrics.
        /// </summary>
        [JsonProperty("executionMetrics")]
        public KustoQueryStats ExecutionMetrics { get; set; }

        /// <summary>
        /// Gets or sets the stacktrace if present.
        /// </summary>
        [JsonProperty("stackTrace")]
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets the main error in case there are nested errors.
        /// </summary>
        [JsonProperty("mainError")]
        public string MainError { get; set; }
    }
}
