// <copyright file="KustoClusterDatabase.cs" company="Microsoft">
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
    public class KustoClusterDatabase : IValidatableObject
    {
        /// <summary>
        /// Gets or sets the kusto cluster.
        /// </summary>
        [JsonProperty("cluster")]
        public string Cluster { get; set; }

        /// <summary>
        /// Gets or sets the kusto database.
        /// </summary>
        [JsonProperty("database")]
        public string Database { get; set; }

        /// <inheritdoc/>
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var isUri = Uri.IsWellFormedUriString(Cluster, UriKind.Absolute);
            if (!isUri)
            {
                yield return new ValidationResult("Cluster needs to be a URL.", new string[] { "cluster" });
            }
            else if (new Uri(Cluster).Scheme != Uri.UriSchemeHttps)
            {
                yield return new ValidationResult("Cluster URL must use https.", new string[] { "cluster" });
            }
        }
    }
}
