// <copyright file="IJsonEntity.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// Interface for a Json Entity.
    /// </summary>
    public interface IJsonEntity
    {
        /// <summary>
        /// Gets the unique identifier for this entity.
        /// </summary>
        [JsonIgnore]
        public string Id { get; }
    }
}
