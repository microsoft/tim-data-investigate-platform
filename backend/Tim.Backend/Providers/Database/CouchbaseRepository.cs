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
    using Couchbase.KeyValue.RangeScan;
    using Serilog;
    using Tim.Backend.Providers.DbModels;

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

            var collectionName = typeof(TJsonEntity).Name;
            Collection = dbClient.CollectionAsync(collectionName);
        }

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
        public async Task<TJsonEntity> AddOrUpdateItem(string id, IJsonEntity entity)
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
#pragma warning disable CA2200 // Rethrow to preserve stack details

                // TODO rethink how we want to do this, or is logging the error once logging is set up enough, no need to throw?
                throw e;
#pragma warning restore CA2200 // Rethrow to preserve stack details
            }
        }

        /// <summary>
        /// Gets an item from an existing collection based on id.
        /// </summary>
        /// <param name="id">Unique condition element is determined by.</param>
        /// <returns>Item if found, null otherwise.</returns>
        public async Task<TJsonEntity> GetItem(string id)
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
#pragma warning disable CA2200 // Rethrow to preserve stack details

                // TODO rethink how we want to do this, or is logging the error once logging is set up enough, no need to throw?
                throw e;
#pragma warning restore CA2200 // Rethrow to preserve stack details
            }
        }

        /// <summary>
        /// Returns all item in a given collection.
        /// </summary>
        /// <returns>List of items if any are present.</returns>
        public async Task<IEnumerable<TJsonEntity>> GetItems()
        {
            try
            {
                var collection = await Collection;
                var queryResult = collection.ScanAsync(new RangeScan(ScanTerm.Minimum(), ScanTerm.Maximum()));
                return await queryResult.Select(result => result.ContentAs<TJsonEntity>()).ToListAsync();
            }
            catch (Exception e)
            {
                Logger.Error(e, "BucketName client failed with error: " + e.ToString());
#pragma warning disable CA2200 // Rethrow to preserve stack details

                // TODO rethink how we want to do this, or is logging the error once logging is set up enough, no need to throw?
                throw e;
#pragma warning restore CA2200 // Rethrow to preserve stack details
            }
        }

        /// <summary>
        /// Deletes items from a given collection.
        /// </summary>
        /// <param name="id">Unique identified for item to delete.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task DeleteItem(string id)
        {
            try
            {
                var collection = await Collection;
                await collection.RemoveAsync(id);
            }
            catch (Exception e)
            {
                Logger.Error(e, "BucketName client failed with error: " + e.ToString());
#pragma warning disable CA2200 // Rethrow to preserve stack details

                // TODO rethink how we want to do this, or is logging the error once logging is set up enough, no need to throw?
                throw e;
#pragma warning restore CA2200 // Rethrow to preserve stack details
            }
        }
    }
}
