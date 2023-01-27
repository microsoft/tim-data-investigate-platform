// <copyright file="JsonEntity.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.DbModels
{
    using Newtonsoft.Json;

    /// <summary>
    /// Abstract JsonEntity to represent database objects.
    /// </summary>
    public class JsonEntity
    {
        /// <summary>
        /// Gets or sets patition key of the object, which is affectively used as a primary key.
        /// </summary>
        [JsonProperty(PropertyName = "_p", NullValueHandling = NullValueHandling.Ignore)]
        public string PartitionKey { get; set; }

        /// <summary>
        /// Gets or sets properties of the given object as Json.
        /// </summary>
        [JsonProperty(PropertyName = "_v", NullValueHandling = NullValueHandling.Ignore)]
        public string Properties { get; set; }
    }
}
