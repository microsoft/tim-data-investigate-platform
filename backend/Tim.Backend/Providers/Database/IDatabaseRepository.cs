// <copyright file="IDatabaseRepository.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Database
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tim.Backend.Models;

    /// <summary>
    /// Add or update an item in the database.
    /// </summary>
    /// <typeparam name="TJsonEntity">Entity type.</typeparam>
    public interface IDatabaseRepository<TJsonEntity>
        where TJsonEntity : IJsonEntity
    {
        /// <summary>
        /// Add or update an item in the database.
        /// </summary>
        /// <param name="id">Identifier for object.</param>
        /// <param name="entity">Object contents.</param>
        /// <returns>Created db object or error.</returns>
        public Task<TJsonEntity> AddOrUpdateItemAsync(string id, IJsonEntity entity);

        /// <summary>
        /// Gets an item from an existing collection based on partition key.
        /// </summary>
        /// <param name="id">Unique identified of the object to retrieve.</param>
        /// <returns>Item if found, null otherwise.</returns>
        public Task<TJsonEntity> GetItemAsync(string id);

        /// <summary>
        /// Returns all item in a given collection.
        /// </summary>
        /// <returns>List of items if any are present.</returns>
        public Task<IEnumerable<TJsonEntity>> GetItemsAsync();

        /// <summary>
        /// Deletes items from a given collection.
        /// </summary>
        /// <param name="id">Unique identified for item to delete.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task DeleteItemAsync(string id);
    }
}
