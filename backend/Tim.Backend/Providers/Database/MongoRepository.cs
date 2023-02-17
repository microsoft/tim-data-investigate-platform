// <copyright file="MongoRepository.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using Serilog;
    using Tim.Backend.Models;

    /// <summary>
    /// Repository of operations for Mongodb.
    /// </summary>
    /// <typeparam name="TJsonEntity">Entity type.</typeparam>
    public class MongoRepository<TJsonEntity> : IDatabaseRepository<TJsonEntity>
        where TJsonEntity : IJsonEntity
    {
        private readonly ILogger m_logger;
        private readonly MongoDbClient m_client;
        private readonly IMongoCollection<TJsonEntity> m_collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoRepository{TJsonEntity}"/> class.
        /// </summary>
        /// <param name="dbClient">Mongodb client.</param>
        public MongoRepository(MongoDbClient dbClient)
        {
            m_logger = Log.Logger;
            m_client = dbClient;

            var collectionName = MongoDbClient.GetCollectionName<TJsonEntity>();
            m_collection = m_client.Database.GetCollection<TJsonEntity>(collectionName);
        }

        /// <inheritdoc/>
        public async Task AddOrUpdateItemAsync(IJsonEntity entity, TimeSpan? timeToLive = null)
        {
            try
            {
                await m_collection
                    .ReplaceOneAsync(
                        x => x.Id == entity.Id,
                        (TJsonEntity)entity,
                        new ReplaceOptions { IsUpsert = true });
            }
            catch (Exception e)
            {
                m_logger.Error(e, "MongoDbClient client failed with error: " + e.ToString());
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TJsonEntity> GetItemAsync(string id)
        {
            try
            {
                return await m_collection
                    .Find(x => x.Id == id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                m_logger.Error(e, "MongoDbClient client failed with error: " + e.ToString());
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TJsonEntity>> GetItemsAsync()
        {
            try
            {
                var filter = Builders<TJsonEntity>.Filter.Empty;
                var projection = Builders<TJsonEntity>.Projection.Exclude("_id");
                return await m_collection
                    .Find(filter)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                m_logger.Error(e, "MongoDbClient client failed with error: " + e.ToString());
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task DeleteItemAsync(string id)
        {
            try
            {
                await m_collection
                    .DeleteOneAsync(x => x.Id == id);
            }
            catch (Exception e)
            {
                m_logger.Error(e, "MongoDbClient client failed with error: " + e.ToString());
                throw;
            }
        }
    }
}