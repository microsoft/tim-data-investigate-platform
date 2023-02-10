// <copyright file="IDatabaseClient.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Database
{
    using System.Threading.Tasks;

    /// <summary>
    /// Database interface.
    /// </summary>
    public interface IDatabaseClient
    {
        /// <summary>
        /// Creates a connection to the database.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task Connect();

        /// <summary>
        /// Execute any initialize steps for the database.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task Initialize();
    }
}
