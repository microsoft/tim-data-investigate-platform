// <copyright file="KustoTriggerRequest.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.KustoQuery.Api
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// Request to trigger a query.
    /// </summary>
    [JsonObject]
    public sealed class KustoTriggerRequest
    {
        /// <summary>
        /// Gets or sets the authenticated user making the request.
        /// </summary>
        [JsonProperty("requestedBy")]
        public string RequestedBy { get; set; }

        /// <summary>
        /// Gets or sets the StartTime of the query window to be executed.
        /// </summary>
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end of the query windows to be executed.
        /// </summary>
        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the query id to be executed.
        /// </summary>
        [JsonProperty("queryId")]
        public string QueryId { get; set; }

        /// <summary>
        /// Validates the arguments in the request and throws an exception when error is found.
        /// </summary>
        /// <exception cref="ArgumentException">Throws if any argument is invalid.</exception>
        public void Validate()
        {
            if (string.IsNullOrEmpty(QueryId))
            {
                throw new ArgumentException("Argument must be specified", nameof(QueryId));
            }

            if (string.IsNullOrEmpty(RequestedBy))
            {
                throw new ArgumentException("Argument must be specified", nameof(RequestedBy));
            }

            if (StartTime < EndTime)
            {
                throw new ArgumentException("End Time has to be greater then start time to execute query");
            }
        }
    }
}
