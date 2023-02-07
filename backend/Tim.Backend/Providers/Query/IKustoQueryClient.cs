// <copyright file="IKustoQueryClient.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Query
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Tim.Backend.Models.KustoQuery;

    /// <summary>
    /// Interface for Kusto client.
    /// </summary>
    public interface IKustoQueryClient
    {
        /// <summary>
        /// Execute a kusto query and return the results.
        /// </summary>
        /// <param name="kustoQuery">Kusto query details.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>Results of the query.</returns>
        Task<KustoQueryResults<IDictionary<string, object>>> RunQuery(KustoQuery kustoQuery, CancellationToken cancellationToken);
    }
}