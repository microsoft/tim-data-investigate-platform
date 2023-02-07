// <copyright file="IDatabaseRepository.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Database
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tim.Backend.Models;

    /// <summary>
    /// Database repository of common CRUD operations.
    /// </summary>
    /// <typeparam name="TJsonEntity">Entity type.</typeparam>
    public interface IDatabaseRepository<TJsonEntity>
        where TJsonEntity : IJsonEntity
    {
        /// <summary>
        /// Add or update object in the database.
        /// </summary>
        /// <param name="entity">Object contents.</param>
        /// <param name="timeToLive">Object time to live.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task AddOrUpdateItemAsync(IJsonEntity entity, TimeSpan? timeToLive = null);

        /// <summary>
        /// Gets an object with the specified id.
        /// </summary>
        /// <param name="id">Identifier for object.</param>
        /// <returns>Object if found, null otherwise.</returns>
        public Task<TJsonEntity> GetItemAsync(string id);

        /// <summary>
        /// Returns all objects based on the entity type.
        /// </summary>
        /// <returns>List of items if any are present.</returns>
        public Task<IEnumerable<TJsonEntity>> GetItemsAsync();

        /// <summary>
        /// Deletes object from database.
        /// </summary>
        /// <param name="id">Identifier for object.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task DeleteItemAsync(string id);
    }
}
