// <copyright file="CouchbaseRepository.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Couchbase.Core.Exceptions.KeyValue;
    using Couchbase.KeyValue;
    using Serilog;
    using Tim.Backend.Models;

    /// <summary>
    /// Outlines the BucketName Client used byt the service.
    /// </summary>
    /// <typeparam name="TJsonEntity">Entity type.</typeparam>
    public class CouchbaseRepository<TJsonEntity> : IDatabaseRepository<TJsonEntity>
        where TJsonEntity : IJsonEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseRepository{TJsonEntity}"/> class.
        /// </summary>
        /// <param name="dbClient">Collection/table names.</param>
        public CouchbaseRepository(CouchbaseDbClient dbClient)
        {
            Logger = Log.Logger;

            DatabaseClient = dbClient;
            Collection = DatabaseClient.CollectionAsync<TJsonEntity>();
        }

        /// <summary>
        /// Gets or sets the Couchbase db client.
        /// </summary>
        public CouchbaseDbClient DatabaseClient { get; set; }

        /// <summary>
        /// Gets or sets the Couchbase db client.
        /// </summary>
        public Task<ICouchbaseCollection> Collection { get; set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Add or update an item in the database.
        /// </summary>
        /// <param name="id">Unique identified of the object.</param>
        /// <param name="entity">Object contents.</param>
        /// <returns>Created db object or error.</returns>
        public async Task<TJsonEntity> AddOrUpdateItemAsync(string id, IJsonEntity entity)
        {
            try
            {
                var collection = await Collection;
                var upsertResult = await collection.UpsertAsync(id, entity);
                var getResult = await collection.GetAsync(id);

                return getResult.ContentAs<TJsonEntity>();
            }
            catch (Exception e)
            {
                Logger.Error(e, "BucketName client failed with error: " + e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Gets an item from an existing collection based on id.
        /// </summary>
        /// <param name="id">Unique condition element is determined by.</param>
        /// <returns>Item if found, null otherwise.</returns>
        public async Task<TJsonEntity> GetItemAsync(string id)
        {
            try
            {
                var collection = await Collection;
                var getResult = await collection.GetAsync(id);

                return getResult.ContentAs<TJsonEntity>();
            }
            catch (DocumentNotFoundException e)
            {
                // element not found
                Logger.Error(e, $"Not able to find element with id {id}: " + e.ToString());
                return default;
            }
            catch (Exception e)
            {
                Logger.Error(e, "BucketName client failed with error: " + e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Returns all item in a given collection.
        /// </summary>
        /// <returns>List of items if any are present.</returns>
        public async Task<IEnumerable<TJsonEntity>> GetItemsAsync()
        {
            try
            {
                var bucket = DatabaseClient.Bucket;
                var scope = await bucket.DefaultScopeAsync();
                var collectionName = CouchbaseDbClient.GetCollectionName<TJsonEntity>();
                var queryResult = await scope.QueryAsync<TJsonEntity>($"SELECT d.* FROM {collectionName} d");

                return await queryResult.Rows.ToListAsync();
            }
            catch (Exception e)
            {
                Logger.Error(e, "BucketName client failed with error: " + e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Deletes items from a given collection.
        /// </summary>
        /// <param name="id">Unique identified for item to delete.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task DeleteItemAsync(string id)
        {
            try
            {
                var collection = await Collection;
                await collection.RemoveAsync(id);
            }
            catch (Exception e)
            {
                Logger.Error(e, "BucketName client failed with error: " + e.ToString());
                throw;
            }
        }
    }
}