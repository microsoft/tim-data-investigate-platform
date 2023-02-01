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
    using Serilog;
    using Tim.Backend.Startup.Config;

    /// <summary>
    /// Outlines the BucketName Client used byt the service.
    /// </summary>
    public class CouchbaseDbClient : IDatabaseClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseDbClient"/> class.
        /// </summary>
        /// <param name="dbConfigs">Collection/table names.</param>
        public CouchbaseDbClient(DatabaseConfiguration dbConfigs)
        {
            Logger = Log.Logger;

            BucketName = dbConfigs.BucketName;
            Configs = dbConfigs;
        }

        /// <summary>
        /// Gets or sets the Couchbase db client.
        /// </summary>
        public ICluster CouchBaseClient { get; set; }

        /// <summary>
        /// Gets the Couchbase bucket client is connected to.
        /// </summary>
        public string BucketName { get; }

        /// <summary>
        /// Gets or sets the db dbConfigs.
        /// </summary>
        public DatabaseConfiguration Configs { get; set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Create a connection to the database.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task Connect()
        {
            Logger.Information($"CouchbaseDbClient is starting up for bucket {Configs.BucketName}. ");
            CouchBaseClient = await Cluster.ConnectAsync(
                Configs.DatabaseConnection,
                new ClusterOptions
                {
                    UserName = Configs.DbUserName,
                    Password = Configs.DbUserPassword,
                    KvTimeout = TimeSpan.FromSeconds(10),
                });
            Logger.Information($"CouchbaseDbClient has been created.");
        }

        /// <summary>
        /// Test.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task Initialize()
        {
            await CouchBaseClient.WaitUntilReadyAsync(TimeSpan.FromMinutes(2));
            await CreateBucketIfNotExists(BucketName);
        }

        /// <summary>
        /// Connect to a couchbase collection.
        /// </summary>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ICouchbaseCollection> CollectionAsync(string collectionName)
        {
            var bucket = await CouchBaseClient.BucketAsync(BucketName);
            return await CreateCollectionIfNotExists(collectionName, bucket);
        }

        private async Task<IBucket> CreateBucketIfNotExists(string bucketName)
        {
            var buckets = await CouchBaseClient.Buckets.GetAllBucketsAsync();
            if (!buckets.ContainsKey(bucketName))
            {
                await CouchBaseClient.Buckets.CreateBucketAsync(new BucketSettings
                {
                    BucketType = BucketType.Couchbase,
                    Name = bucketName,
                    NumReplicas = 0,
                    RamQuotaMB = 100,
                });
            }

            var bucket = await CouchBaseClient.BucketAsync(bucketName);
            await bucket.WaitUntilReadyAsync(TimeSpan.FromMinutes(2));
            return bucket;
        }

        private async Task<ICouchbaseCollection> CreateCollectionIfNotExists(string collectionName, IBucket bucket)
        {
            var scope = await bucket.DefaultScopeAsync();
            var collectionSpec = new CollectionSpec(scope.Name, collectionName);

            try
            {
                await bucket.Collections.CreateCollectionAsync(collectionSpec);
            }
            catch (CollectionExistsException)
            {
                Logger.Information($"Collection {collectionName} already exists.");
            }

            return await bucket.CollectionAsync(collectionName);
        }
    }
}
