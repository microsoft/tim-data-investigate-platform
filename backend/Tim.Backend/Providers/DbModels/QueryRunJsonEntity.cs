// <copyright file="QueryRunJsonEntity.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.DbModels
{
    using Newtonsoft.Json;
    using Tim.Backend.Models.KustoQuery;

    /// <summary>
    /// Object that represents the Saved Query Run that gets read from the database.
    /// </summary>
    public class QueryRunJsonEntity : IJsonEntity
    {
        /// <summary>
        /// Gets or sets the saved query run metadata.
        /// </summary>
        [JsonProperty(PropertyName = "QueryRun")]
        public KustoQueryRun QueryRun { get; set; }
    }
}
