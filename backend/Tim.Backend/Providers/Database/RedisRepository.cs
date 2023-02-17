// <copyright file="RedisRepository.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Serilog;
    using StackExchange.Redis;
    using Tim.Backend.Models;

    /// <summary>
    /// Repository of operations for Redis.
    /// </summary>
    /// <typeparam name="TJsonEntity">Entity type.</typeparam>
    public class RedisRepository<TJsonEntity> : IDatabaseRepository<TJsonEntity>
        where TJsonEntity : IJsonEntity
    {
        private readonly ILogger m_logger;
        private readonly RedisDbClient m_client;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisRepository{TJsonEntity}"/> class.
        /// </summary>
        /// <param name="dbClient">Redis client.</param>
        public RedisRepository(RedisDbClient dbClient)
        {
            m_logger = Log.Logger;
            m_client = dbClient;
        }

        /// <inheritdoc/>
        public async Task AddOrUpdateItemAsync(IJsonEntity entity, TimeSpan? timeToLive = null)
        {
            try
            {
                var db = m_client.Connection.GetDatabase();
                var key = GetAsKey(entity.Id);
                await db.StringSetAsync(key, JsonConvert.SerializeObject(entity));
            }
            catch (Exception e)
            {
                m_logger.Error(e, "RedisDbClient client failed with error: " + e.ToString());
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<TJsonEntity> GetItemAsync(string id)
        {
            try
            {
                var db = m_client.Connection.GetDatabase();
                var key = GetAsKey(id);
                var result = await db.StringGetAsync(key);
                return result.HasValue ? JsonConvert.DeserializeObject<TJsonEntity>(result) : default;
            }
            catch (Exception e)
            {
                m_logger.Error(e, "RedisDbClient client failed with error: " + e.ToString());
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TJsonEntity>> GetItemsAsync()
        {
            try
            {
                var items = new List<TJsonEntity>();
                var db = m_client.Connection.GetDatabase();

                foreach (var server in GetAllServers())
                {
                    var keys = await server.KeysAsync(pattern: $"{typeof(TJsonEntity).Name}/*").ToArrayAsync();
                    var values = await db.StringGetAsync(keys);
                    items.AddRange(values.Select(value => JsonConvert.DeserializeObject<TJsonEntity>(value)));
                }

                return items;
            }
            catch (Exception e)
            {
                m_logger.Error(e, "RedisDbClient client failed with error: " + e.ToString());
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task DeleteItemAsync(string id)
        {
            try
            {
                var db = m_client.Connection.GetDatabase();
                var key = GetAsKey(id);
                await db.StringGetDeleteAsync(key);
            }
            catch (Exception e)
            {
                m_logger.Error(e, "RedisDbClient client failed with error: " + e.ToString());
                throw;
            }
        }

        private static string GetAsKey(string id)
        {
            return $"{typeof(TJsonEntity).Name}/{id}";
        }

        private IEnumerable<IServer> GetAllServers()
        {
            var endpoints = m_client.Connection.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                yield return m_client.Connection.GetServer(endpoint);
            }
        }
    }
}