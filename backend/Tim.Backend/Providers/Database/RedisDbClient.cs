// <copyright file="RedisDbClient.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Database
{
    using System.Threading.Tasks;
    using Serilog;
    using StackExchange.Redis;
    using Tim.Backend.Models;
    using Tim.Backend.Startup.Config;

    /// <summary>
    /// Manages the connection and initialization for the couchbase database.
    /// </summary>
    public class RedisDbClient : IDatabaseClient
    {
        private readonly ILogger m_logger;
        private readonly RedisConfiguration m_configs;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisDbClient"/> class.
        /// </summary>
        /// <param name="dbConfigs">Collection/table names.</param>
        public RedisDbClient(RedisConfiguration dbConfigs)
        {
            m_logger = Log.Logger;
            m_configs = dbConfigs;
        }

        /// <summary>
        /// Gets or sets the connection to redis cluster.
        /// </summary>
        public ConnectionMultiplexer Connection { get; set; }

        /// <summary>
        /// Generate the collection name for this entity type.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public static string GetCollectionName<T>()
            where T : IJsonEntity
        {
            return typeof(T).Name;
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            m_logger.Information("Initializing redis.");
            Connection = await ConnectionMultiplexer.ConnectAsync(m_configs.ConnectionString);
        }
    }
}
