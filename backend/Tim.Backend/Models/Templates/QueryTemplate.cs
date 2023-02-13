// <copyright file="QueryTemplate.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.Templates
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines query types.
    /// </summary>
    public enum QueryType
    {
        /// <summary>
        /// Kusto query view.
        /// </summary>
        View,

        /// <summary>
        /// Kusto query.
        /// </summary>
        Query,
    }

    /// <summary>
    /// Class that defines query template.
    /// </summary>
    public class QueryTemplate : IValidatableObject, IJsonEntity
    {
        /// <inheritdoc/>
        public string Id => Uuid.ToString();

        /// <summary>
        /// Gets or sets the uuid for a given query template.
        /// </summary>
        [Required]
        [JsonProperty("uuid")]
        public Guid Uuid { get; set; }

        /// <summary>
        /// Gets or sets the name for the query template.
        /// </summary>
        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether query template is deleted or not.
        /// </summary>
        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether query template is managed or not.
        /// </summary>
        [JsonProperty("isManaged")]
        public bool IsManaged { get; set; }

        /// <summary>
        /// Gets or sets when query template was last updated.
        /// </summary>
        [JsonProperty("updated")]
        public DateTime Updated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets who created the query template.
        /// </summary>
        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets who last updated the query template.
        /// </summary>
        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets the query type.
        /// </summary>
        [Required]
        [JsonProperty("queryType")]
        public QueryType QueryType { get; set; }

        /// <summary>
        /// Gets or sets where in the menu the query template will appear.
        /// </summary>
        [Required]
        [JsonProperty("menu")]
        public string Menu { get; set; }

        /// <summary>
        /// Gets or sets the summary for query template.
        /// </summary>
        [Required]
        [JsonProperty("summary")]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the path for query template.
        /// </summary>
        [Required]
        [JsonProperty("path")]
        public IEnumerable<string> Path { get; set; }

        /// <summary>
        /// Gets or sets the cluster for query template.
        /// </summary>
        [Required]
        [JsonProperty("cluster")]
        public string Cluster { get; set; }

        /// <summary>
        /// Gets or sets the database for query template.
        /// </summary>
        [Required]
        [JsonProperty("database")]
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the column id.
        /// </summary>
        [JsonProperty("columnId")]
        public string ColumnId { get; set; }

        /// <summary>
        /// Gets or sets the query template params.
        /// </summary>
        [JsonProperty("params")]
        public Dictionary<string, QueryParam> Params { get; set; }

        /// <summary>
        /// Gets or sets the query template fields.
        /// </summary>
        [JsonProperty("fields")]
        public Dictionary<string, QueryField> Fields { get; set; }

        /// <summary>
        /// Gets or sets the query template columns.
        /// </summary>
        [JsonProperty("columns")]
        public Dictionary<string, object> Columns { get; set; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        [Required]
        [JsonProperty("query")]
        public string Query { get; set; }

        /// <summary>
        /// Validate that all required fields are present as expected.
        /// </summary>
        /// <param name="validationContext">validation context.</param>
        /// <returns>Any located errors.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (QueryType == QueryType.Query && (Fields == null || !Fields.Any()))
            {
                yield return new ValidationResult("'fields' field is required if query type is 'query'");
            }
        }
    }
}
