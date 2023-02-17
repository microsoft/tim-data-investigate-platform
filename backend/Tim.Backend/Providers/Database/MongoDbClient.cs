// <copyright file="MongoDbClient.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Database
{
    using System;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Serilog;
    using Tim.Backend.Models;
    using Tim.Backend.Models.KustoQuery;
    using Tim.Backend.Models.Templates;
    using Tim.Backend.Startup.Config;

    /// <summary>
    /// Manages the connection and initialization for the couchbase database.
    /// </summary>
    public class MongoDbClient : IDatabaseClient
    {
        private readonly ILogger m_logger;
        private readonly MongoConfiguration m_configs;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDbClient"/> class.
        /// </summary>
        /// <param name="dbConfigs">Collection/table names.</param>
        public MongoDbClient(MongoConfiguration dbConfigs)
        {
            m_logger = Log.Logger;
            m_configs = dbConfigs;
        }

        /// <summary>
        /// Gets or sets the connection to mongo database.
        /// </summary>
        public IMongoDatabase Database { get; set; }

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
            m_logger.Information("Initializing mongo.");

            Database = new MongoClient(m_configs.ConnectionString)
                .GetDatabase(m_configs.DatabaseName);

            await CreateCollectionAsync(GetCollectionName<KustoQueryRun>());
            await CreateCollectionAsync(GetCollectionName<QueryTemplate>());

            // TODO: This method should be more generic
            await CreateKustoExpireIndexKustoQueryRun(GetCollectionName<KustoQueryRun>());
        }

        private async Task CreateCollectionAsync(string collectionName)
        {
            try
            {
                m_logger.Information($"Creating collection {collectionName}.");
                await Database.CreateCollectionAsync(collectionName);
            }
            catch (MongoCommandException ex)
            {
                if (ex.Code == 48)
                {
                    m_logger.Information($"Collection {collectionName} already exists.");
                }
                else
                {
                    throw ex;
                }
            }
        }

        private async Task CreateKustoExpireIndexKustoQueryRun(string collectionName)
        {
            var builder = Builders<KustoQueryRun>.IndexKeys;
            var keys = m_configs.WithCosmosDb ? builder.Ascending("_ts") : builder.Ascending(x => x.ExecuteDateTimeUtc);
            var indexModel = new CreateIndexModel<KustoQueryRun>(
                keys,
                new CreateIndexOptions
                {
                    ExpireAfter = TimeSpan.FromDays(1),
                });

            m_logger.Information($"Creating expire index for collection {collectionName}.");
            await Database.GetCollection<KustoQueryRun>(collectionName).Indexes
                .CreateOneAsync(indexModel);
        }
    }
}
