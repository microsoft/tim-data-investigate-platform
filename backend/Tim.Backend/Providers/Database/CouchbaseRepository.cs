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
    /// Repository of operations for Couchbase.
    /// </summary>
    /// <typeparam name="TJsonEntity">Entity type.</typeparam>
    public class CouchbaseRepository<TJsonEntity> : IDatabaseRepository<TJsonEntity>
        where TJsonEntity : IJsonEntity
    {
        private readonly ILogger m_logger;
        private readonly CouchbaseDbClient m_client;
        private readonly Task<ICouchbaseCollection> m_collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseRepository{TJsonEntity}"/> class.
        /// </summary>
        /// <param name="dbClient">Couchbase client.</param>
        public CouchbaseRepository(CouchbaseDbClient dbClient)
        {
            m_logger = Log.Logger;
            m_client = dbClient;
            m_collection = m_client.CollectionAsync<TJsonEntity>();
        }

        /// <inheritdoc/>
        public async Task AddOrUpdateItemAsync(IJsonEntity entity, TimeSpan? timeToLive = null)
        {
            var upsertOptions = new UpsertOptions();

            if (timeToLive != null)
            {
                upsertOptions.Expiry(timeToLive.Value);
            }

            try
            {
                var collection = await m_collection;
                var upsertResult = await collection.UpsertAsync(entity.Id, entity, upsertOptions);
            }
            catch (Exception e)
            {
                m_logger.Error(e, "CouchbaseDbClient client failed with error: " + e.ToString());
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TJsonEntity> GetItemAsync(string id)
        {
            try
            {
                var collection = await m_collection;
                var getResult = await collection.GetAsync(id);

                return getResult.ContentAs<TJsonEntity>();
            }
            catch (DocumentNotFoundException e)
            {
                // element not found
                m_logger.Error(e, $"Not able to find element with id {id}: " + e.ToString());
                return default;
            }
            catch (Exception e)
            {
                m_logger.Error(e, "CouchbaseDbClient client failed with error: " + e.ToString());
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TJsonEntity>> GetItemsAsync()
        {
            try
            {
                var bucket = m_client.Bucket;
                var scope = await bucket.DefaultScopeAsync();
                var collectionName = CouchbaseDbClient.GetCollectionName<TJsonEntity>();
                var queryResult = await scope.QueryAsync<TJsonEntity>($"SELECT d.* FROM {collectionName} d");

                return await queryResult.Rows.ToListAsync();
            }
            catch (Exception e)
            {
                m_logger.Error(e, "CouchbaseDbClient client failed with error: " + e.ToString());
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task DeleteItemAsync(string id)
        {
            try
            {
                var collection = await m_collection;
                await collection.RemoveAsync(id);
            }
            catch (Exception e)
            {
                m_logger.Error(e, "CouchbaseDbClient client failed with error: " + e.ToString());
                throw;
            }
        }
    }
}