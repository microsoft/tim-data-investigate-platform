// <copyright file="CouchbaseDbClient.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Database
{
    using System;
    using System.Threading.Tasks;
    using Couchbase;
    using Couchbase.KeyValue;
    using Couchbase.Management.Buckets;
    using Couchbase.Management.Collections;
    using Couchbase.Management.Query;
    using Serilog;
    using Tim.Backend.Models;
    using Tim.Backend.Models.KustoQuery;
    using Tim.Backend.Models.Templates;
    using Tim.Backend.Startup.Config;

    /// <summary>
    /// Manages the connection and initialization for the couchbase database.
    /// </summary>
    public class CouchbaseDbClient : IDatabaseClient
    {
        private const int c_retryMultipler = 500;
        private static readonly TimeSpan s_initializationTimeout = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan s_queryTimeout = TimeSpan.FromSeconds(10);
        private readonly ILogger m_logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseDbClient"/> class.
        /// </summary>
        /// <param name="dbConfigs">Collection/table names.</param>
        public CouchbaseDbClient(DatabaseConfiguration dbConfigs)
        {
            m_logger = Log.Logger;
            Configs = dbConfigs;
        }

        /// <summary>
        /// Gets or sets the Couchbase db client.
        /// </summary>
        public ICluster CouchBaseClient { get; set; }

        /// <summary>
        /// Gets or sets the database configs.
        /// </summary>
        public DatabaseConfiguration Configs { get; set; }

        /// <summary>
        /// Gets or sets the bucket after initialization.
        /// </summary>
        public IBucket Bucket { get; set; }

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
        public async Task ConnectAsync()
        {
            m_logger.Information($"CouchbaseDbClient is connecting to database.");
            CouchBaseClient = await Cluster.ConnectAsync(
                Configs.DatabaseConnection,
                new ClusterOptions
                {
                    UserName = Configs.DbUserName,
                    Password = Configs.DbUserPassword,
                    KvTimeout = s_queryTimeout,
                });
            m_logger.Information($"CouchbaseDbClient has been created.");
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            if (CouchBaseClient is null)
            {
                throw new InvalidOperationException("CouchBaseClient must be initialized with a connection.");
            }

            await CouchBaseClient.WaitUntilReadyAsync(s_initializationTimeout);
            Bucket = await CreateBucketIfNotExists(Configs.BucketName);
            await CreateCollectionIfNotExists<KustoQueryRun>();
            await CreateCollectionIfNotExists<QueryTemplate>();
        }

        /// <summary>
        /// Connect to a couchbase collection.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ICouchbaseCollection> CollectionAsync<T>()
            where T : IJsonEntity
        {
            if (Bucket is null)
            {
                throw new InvalidOperationException("Bucket must be initialized.");
            }

            var collectionName = GetCollectionName<T>();
            return await Bucket.CollectionAsync(collectionName);
        }

        /// <summary>
        /// Get the default scope name for this bucket.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<string> GetScopeName()
        {
            var scope = await Bucket.DefaultScopeAsync();
            return scope.Name;
        }

        private async Task CreateCollectionIfNotExists<T>()
            where T : IJsonEntity
        {
            if (Bucket is null)
            {
                throw new InvalidOperationException("Bucket must be initialized.");
            }

            var collectionName = GetCollectionName<T>();
            var scopeName = await GetScopeName();
            var collectionSpec = new CollectionSpec(scopeName, collectionName);

            try
            {
                var retries = 0;
                while (retries++ < 3)
                {
                    m_logger.Information($"Creating new collection {collectionName}.");
                    await Bucket.Collections.CreateCollectionAsync(collectionSpec);
                    await Task.Delay(TimeSpan.FromMilliseconds(retries * c_retryMultipler));
                }
            }
            catch (CollectionExistsException)
            {
                m_logger.Information($"Collection {collectionName} already exists.");
            }

            m_logger.Information($"Creating primary index for collection {collectionName}.");
            await CouchBaseClient.QueryIndexes.CreatePrimaryIndexAsync(
                Bucket.Name,
                new CreatePrimaryQueryIndexOptions()
                    .ScopeName(scopeName)
                    .CollectionName(collectionName)
                    .IgnoreIfExists(true));
        }

        private async Task<IBucket> CreateBucketIfNotExists(string bucketName)
        {
            if (CouchBaseClient is null)
            {
                throw new InvalidOperationException("CouchBaseClient must be initialized with a connection.");
            }

            var buckets = await CouchBaseClient.Buckets.GetAllBucketsAsync();
            if (!buckets.ContainsKey(bucketName))
            {
                m_logger.Information($"Creating bucket {bucketName}.");

                // TODO: This should be configured by the couchbase server and not hard coded here
                await CouchBaseClient.Buckets.CreateBucketAsync(new BucketSettings
                {
                    BucketType = BucketType.Couchbase,
                    Name = bucketName,
                    NumReplicas = 0,
                    RamQuotaMB = 100,
                });
            }

            var bucket = await CouchBaseClient.BucketAsync(bucketName);

            return bucket;
        }
    }
}
