// <copyright file="QueryTemplateJsonEntity.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.DbModels
{
    using Newtonsoft.Json;
    using Tim.Backend.Models.Templates;

    /// <summary>
    /// Object that represents the Query templates that gets read from the database.
    /// </summary>
    public class QueryTemplateJsonEntity : JsonEntity
    {
        /// <summary>
        /// Gets or Sets the Query template metadata.
        /// </summary>
        [JsonProperty(PropertyName = "QueryTemplate")]
        public QueryTemplate QueryTemplate { get; set; }
    }
}
