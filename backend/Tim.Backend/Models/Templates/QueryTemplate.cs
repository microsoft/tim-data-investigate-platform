// <copyright file="QueryTemplate.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.Templates
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

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
        /// <summary>
        /// Gets or sets the uuid for a given query template.
        /// </summary>
        [Required]
        public Guid Uuid { get; set; }

        /// <summary>
        /// Gets or sets the name for the query template.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether query template is deleted or not.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether query template is managed or not.
        /// </summary>
        public bool IsManaged { get; set; }

        /// <summary>
        /// Gets or sets when query template was last updated.
        /// </summary>
        public DateTime Updated { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets who created the query template.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets who last updated the query template.
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets the query type.
        /// </summary>
        [Required]
        public QueryType QueryType { get; set; }

        /// <summary>
        /// Gets or sets where in the menu the query template will appear.
        /// </summary>
        [Required]
        public string Menu { get; set; }

        /// <summary>
        /// Gets or sets the summary for query template.
        /// </summary>
        [Required]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the path for query template.
        /// </summary>
        [Required]
        public IEnumerable<string> Path { get; set; }

        /// <summary>
        /// Gets or sets the cluster for query template.
        /// </summary>
        [Required]
        public string Cluster { get; set; }

        /// <summary>
        /// Gets or sets the database for query template.
        /// </summary>
        [Required]
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the column id.
        /// </summary>
        public string ColumnId { get; set; }

        /// <summary>
        /// Gets or sets the query template params.
        /// </summary>
        public Dictionary<string, QueryParam> Params { get; set; }

        /// <summary>
        /// Gets or sets the query template fields.
        /// </summary>
        public Dictionary<string, QueryField> Fields { get; set; }

        /// <summary>
        /// Gets or sets the query template columns.
        /// </summary>
        public Dictionary<string, object> Columns { get; set; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        [Required]
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
