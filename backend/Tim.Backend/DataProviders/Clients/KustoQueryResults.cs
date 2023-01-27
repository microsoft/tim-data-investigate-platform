// <copyright file="KustoQueryResults.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.DataProviders.Clients
{
    using System.Collections.Generic;

    /// <summary>
    /// Kusto Query Results.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    public class KustoQueryResults<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KustoQueryResults{T}"/> class.
        /// </summary>
        /// <param name="queryResults">List of results.</param>
        /// <param name="stats">Query stats.</param>
        public KustoQueryResults(IEnumerable<T> queryResults, KustoQueryStats stats)
        {
            QueryResults = queryResults;
            Stats = stats;
        }

        /// <summary>
        /// Gets query results.
        /// </summary>
        public IEnumerable<T> QueryResults { get; }

        /// <summary>
        /// Gets or sets query stats.
        /// </summary>
        public KustoQueryStats Stats { get; set; }
    }
}
