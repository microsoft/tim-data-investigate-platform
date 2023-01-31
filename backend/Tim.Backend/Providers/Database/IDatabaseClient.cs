// <copyright file="IDatabaseClient.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Database
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Database interface.
    /// </summary>
    public interface IDatabaseClient
    {
        /// <summary>
        /// Creates a connection to the database.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task ConnectDatabase();

        /// <summary>
        /// Generic add or update an item in a collection.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="partitionKey">Identifier of item.</param>
        /// <param name="jsonObject">Json representation of the item.</param>
        /// <param name="collectionName">Which collection to use.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<T> AddOrUpdateItem<T>(string partitionKey, string jsonObject, string collectionName)
            where T : class;

        /// <summary>
        /// Generic get an item from a collection.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="partitionKey">Identifier of item.</param>
        /// <param name="collectionName">Which collection to use.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<T> GetItem<T>(string partitionKey, string collectionName)
            where T : class;

        /// <summary>
        /// Generic delete an item from a collection.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="partitionKey">Identifier of item.</param>
        /// <param name="collectionName">Which collection to use.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task DeleteItem<T>(string partitionKey, string collectionName)
            where T : class;

        /// <summary>
        /// Generic get multiple items from a collection.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="collectionName">Which collection to use.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<T>> GetItems<T>(string collectionName)
            where T : class;
    }
}
