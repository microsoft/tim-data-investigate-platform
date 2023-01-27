// <copyright file="KustoQueryStats.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.DataProviders.Clients
{
    using System;

    using Newtonsoft.Json;

    /// <summary>
    /// Kusto query stats.
    /// </summary>
    public partial class KustoQueryStats
    {
        [JsonProperty("ExecutionTime")]
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1601 // Partial elements should be documented
#pragma warning disable SA1402 // File may only contain a single type
        public double ExecutionTime { get; set; }

        [JsonProperty("resource_usage")]
        public ResourceUsage ResourceUsage { get; set; }

        [JsonProperty("input_dataset_statistics")]
        public InputDatasetStatistics InputDatasetStatistics { get; set; }

        [JsonProperty("dataset_statistics")]
        public DatasetStatistic[] DatasetStatistics { get; set; }
    }

#pragma warning disable SA1601 // Partial elements should be documented
    public partial class DatasetStatistic
    {
        [JsonProperty("table_row_count")]
        public long TableRowCount { get; set; }

        [JsonProperty("table_size")]
        public long TableSize { get; set; }
    }

    public partial class InputDatasetStatistics
    {
        [JsonProperty("extents")]
        public ExtentStats ExtentsStats { get; set; }

        [JsonProperty("rows")]
        public RowStats Rows { get; set; }

        [JsonProperty("rowstores")]
        public Rowstores Rowstores { get; set; }

        [JsonProperty("shards")]
        public InputDatasetStatisticsShards Shards { get; set; }
    }

    public partial class ExtentStats
    {
        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("scanned")]
        public long Scanned { get; set; }

        [JsonProperty("scanned_min_datetime")]
        public DateTimeOffset ScannedMinDatetime { get; set; }

        [JsonProperty("scanned_max_datetime")]
        public DateTimeOffset ScannedMaxDatetime { get; set; }
    }

    public partial class RowStats
    {
        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("scanned")]
        public long Scanned { get; set; }
    }

    public partial class Rowstores
    {
        [JsonProperty("scanned_rows")]
        public long ScannedRows { get; set; }

        [JsonProperty("scanned_values_size")]
        public long ScannedValuesSize { get; set; }
    }

    public partial class InputDatasetStatisticsShards
    {
        [JsonProperty("queries_generic")]
        public long QueriesGeneric { get; set; }

        [JsonProperty("queries_specialized")]
        public long QueriesSpecialized { get; set; }
    }

    public partial class ResourceUsage
    {
        [JsonProperty("cache")]
        public ShardCacheStats Cache { get; set; }

        [JsonProperty("cpu")]
        public CpuUsage Cpu { get; set; }

        [JsonProperty("memory")]
        public MemoryStat Memory { get; set; }
    }

    public partial class ShardCacheStats
    {
        [JsonProperty("memory")]
        public CacheStats Memory { get; set; }

        [JsonProperty("disk")]
        public CacheStats Disk { get; set; }

        [JsonProperty("shards")]
        public CacheShards Shards { get; set; }

        [JsonProperty("results_cache_origin")]
        public ResultsCacheOrigin ResultsCache { get; set; }
    }

    public partial class CacheStats
    {
        [JsonProperty("hits")]
        public long Hits { get; set; }

        [JsonProperty("misses")]
        public long Misses { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }
    }

    public partial class CacheShards
    {
        [JsonProperty("hot")]
        public StorageUsageStats HotCacheUsage { get; set; }

        [JsonProperty("cold")]
        public StorageUsageStats ColdStorageUsage { get; set; }

        [JsonProperty("bypassbytes")]
        public long Bypassbytes { get; set; }
    }

    public partial class ResultsCacheOrigin
    {
        [JsonProperty("client_request_id")]
        public string ClientRequestId { get; set; }

        [JsonProperty("started_on")]
        public DateTimeOffset StartedOn { get; set; }
    }

    public partial class StorageUsageStats
    {
        [JsonProperty("hitbytes")]
        public long Hitbytes { get; set; }

        [JsonProperty("missbytes")]
        public long Missbytes { get; set; }

        [JsonProperty("retrievebytes")]
        public long Retrievebytes { get; set; }
    }

    public partial class CpuUsage
    {
        [JsonProperty("user")]
        public TimeSpan User { get; set; }

        [JsonProperty("kernel")]
        public TimeSpan Kernel { get; set; }

        [JsonProperty("total cpu")]
        public TimeSpan TotalCpu { get; set; }
    }

    public partial class MemoryStat
    {
        [JsonProperty("peak_per_node")]
        public long PeakPerNode { get; set; }
    }
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore SA1601 // Partial elements should be documented
#pragma warning restore SA1402 // File may only contain a single type
}
