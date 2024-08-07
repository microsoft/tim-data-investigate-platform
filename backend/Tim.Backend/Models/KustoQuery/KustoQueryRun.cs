// <copyright file="KustoQueryRun.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.KustoQuery
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using MongoDB.Bson.Serialization.Attributes;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Tim.Backend.Providers.Helpers;

    /// <summary>
    /// Query Run states.
    /// </summary>
    public enum QueryRunStatus
    {
        /// <summary>
        /// Query first created.
        /// </summary>
        [EnumMember(Value = "created")]
        Created,

        /// <summary>
        /// Query completed.
        /// </summary>
        [EnumMember(Value = "completed")]
        Completed,

        /// <summary>
        /// Query had an error.
        /// </summary>
        [EnumMember(Value = "error")]
        Error,

        /// <summary>
        /// Query timed out.
        /// </summary>
        [EnumMember(Value = "timedOut")]
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
            Status = QueryRunStatus.Created;
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
        [Required]
        [BsonId]
        [JsonProperty("queryRunId")]
        public Guid QueryRunId { get; set; }

        /// <summary>
        /// Gets or sets the query run status.
        /// </summary>
        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public QueryRunStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the resulting data from the query.
        /// </summary>
        [JsonProperty("resultData")]
        [BsonIgnore]
        public IEnumerable<IDictionary<string, object>> ResultData { get; set; }

        /// <summary>
        /// Gets or sets the result data as a JSON string for databases without proper support.
        /// </summary>
        [JsonIgnore]
        public byte[] ResultDataAsString
        {
            get => ResultData.ObjectToByteArray();
            set => ResultData = value.ObjectFromByteArray<IEnumerable<IDictionary<string, object>>>();
        }

        /// <summary>
        /// Gets or sets the execution metrics.
        /// </summary>
        [JsonProperty("executionMetrics")]
        [BsonIgnore]
        public KustoQueryStats ExecutionMetrics { get; set; }

        /// <summary>
        /// Gets or sets the execution metrics as a JSON string.
        /// </summary>
        [JsonIgnore]
        public string ExecutionMetricsAsString
        {
            get => JsonConvert.SerializeObject(ExecutionMetrics);
            set => ExecutionMetrics = JsonConvert.DeserializeObject<KustoQueryStats>(value);
        }

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
