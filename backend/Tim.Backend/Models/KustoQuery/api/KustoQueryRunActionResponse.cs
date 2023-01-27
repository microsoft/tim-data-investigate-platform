// <copyright file="KustoQueryRunActionResponse.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.KustoQuery.Api
{
    using System;
    using Newtonsoft.Json;
    using Tim.Backend.DataProviders.Clients;

    /// <summary>
    /// Response for query actions.
    /// </summary>
    [JsonObject]
    public class KustoQueryRunActionResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KustoQueryRunActionResponse"/> class.
        /// </summary>
        public KustoQueryRunActionResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KustoQueryRunActionResponse"/> class.
        /// </summary>
        /// <param name="id">Query run Id.</param>
        /// <param name="message">Message if there is an error.</param>
        public KustoQueryRunActionResponse(Guid id, string message)
        {
            QueryRunId = id.ToString();
            Message = message;
        }

        /// <summary>
        /// Gets or sets query run Id.
        /// </summary>
        [JsonProperty("queryRunId")]
        public string QueryRunId { get; set; }

        /// <summary>
        /// Gets or sets message. Will be blank if operaion was succesful.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets query results.
        /// </summary>
        [JsonProperty("results")]
        public object Results { get; set; }

        /// <summary>
        /// Gets or sets the row counr of query results.
        /// </summary>
        [JsonProperty("rowCount")]
        public int? RowCount { get; set; }

        /// <summary>
        /// Gets or sets execution metrics from the query that was executed.
        /// </summary>
        [JsonProperty("executionMetrics")]
        public KustoQueryStats ExecutionMetrics { get; set; }
    }
}
