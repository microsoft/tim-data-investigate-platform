// <copyright file="KustoQueryStats.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.KustoQuery
{
    using Newtonsoft.Json;

    /// <summary>
    /// Kusto query stats.
    /// </summary>
    public partial class KustoQueryStats
    {
        /// <summary>
        /// Gets or sets time taken to execute kusto query.
        /// </summary>
        [JsonProperty("execution_time")]
        public double ExecutionTime { get; set; }

        /// <summary>
        /// Gets or sets stats on resources needed to execute query.
        /// </summary>
        [JsonProperty("resource_usage")]
        public dynamic ResourceUsage { get; set; }

        /// <summary>
        /// Gets or sets stats on the data e.g. extends, shards, etc.
        /// </summary>
        [JsonProperty("input_dataset_statistics")]
        public dynamic InputDatasetStatistics { get; set; }

        /// <summary>
        /// Gets or sets stats on the resultset e.g. row count.
        /// </summary>
        [JsonProperty("dataset_statistics")]
        public dynamic DatasetStatistics { get; set; }
    }
}
