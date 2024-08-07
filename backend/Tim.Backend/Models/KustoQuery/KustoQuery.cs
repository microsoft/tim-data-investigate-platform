// <copyright file="KustoQuery.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.KustoQuery
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    /// <summary>
    /// Request to execute kusto query.
    /// </summary>
    [JsonObject]
    public sealed class KustoQuery : KustoClusterDatabase, IValidatableObject
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
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time for query to be executed.
        /// </summary>
        [JsonProperty("endTime")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        [Required]
        [JsonProperty("query")]
        public string QueryText { get; set; }

        /// <inheritdoc/>
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            base.Validate(validationContext);

            if (StartTime > EndTime)
            {
                yield return new ValidationResult("EndTime must be greater than StartTime.");
            }
        }
    }
}
