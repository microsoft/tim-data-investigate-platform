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
#pragma warning disable SA1600 // Elements should be documented
        Task<T> AddorUpdateItem<T>(string partitionKey, string jsonObject, string collectionName)
            where T : class;

        Task<T> GetItem<T>(string partitionKey, string collectionName)
            where T : class;

        void DeleteItem<T>(string partitionKey, string collectionName)
            where T : class;

        Task<IEnumerable<T>> GetItems<T>(string collectionName)
            where T : class;
#pragma warning restore SA1600 // Elements should be documented
    }
}
