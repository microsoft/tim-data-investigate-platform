// <copyright file="KustoQuery.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.KustoQuery
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// Request to execute kusto query.
    /// </summary>
    [JsonObject]
    public sealed class KustoQuery
    {
        /// <summary>
        /// Gets or sets the authenticated user making the request.
        /// </summary>
        [JsonProperty("requestedBy")]
        public string RequestedBy { get; set; }

        /// <summary>
        /// Gets or sets start time for query to be executed.
        /// </summary>
        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time for query to be executed.
        /// </summary>
        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the cluster the query will be executed in.
        /// </summary>
        [JsonProperty("cluster")]
        public string Cluster { get; set; }

        /// <summary>
        /// Gets or sets the database the query will be executed in.
        /// </summary>
        [JsonProperty("database")]
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        [JsonProperty("query")]
        public string Query { get; set; }

        /// <summary>
        /// Validates the arguments in the request and throws an exception when error is found.
        /// </summary>
        /// <exception cref="ArgumentException">Throws if any argument is invalid.</exception>
        public void Validate()
        {
            if (string.IsNullOrEmpty(Query))
            {
                throw new ArgumentException("Argument must be specified", nameof(Query));
            }

            if (string.IsNullOrEmpty(RequestedBy))
            {
                throw new ArgumentException("Argument must be specified", nameof(RequestedBy));
            }

            if (string.IsNullOrEmpty(Query))
            {
                throw new ArgumentException("Argument must be specified", nameof(Cluster));
            }

            if (string.IsNullOrEmpty(Query))
            {
                throw new ArgumentException("Argument must be specified", nameof(Database));
            }

            if (StartTime > EndTime)
            {
                throw new ArgumentException("End Time has to be greater then start time to execute query");
            }

            // this string should bew able to be parsed into a URI
            var isUri = Uri.IsWellFormedUriString(Cluster, UriKind.RelativeOrAbsolute);
            if (!isUri)
            {
                throw new ArgumentException("Cluster needs to be a URI. ", nameof(Cluster));
            }
        }
    }
}
