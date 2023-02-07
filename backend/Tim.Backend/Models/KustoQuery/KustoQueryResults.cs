// <copyright file="KustoQueryResults.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Models.KustoQuery
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
        /// <param name="queryStats">Query stats.</param>
        public KustoQueryResults(IEnumerable<T> queryResults, KustoQueryStats queryStats)
        {
            QueryResults = queryResults;
            QueryStats = queryStats;
        }

        /// <summary>
        /// Gets query results.
        /// </summary>
        public IEnumerable<T> QueryResults { get; }

        /// <summary>
        /// Gets or sets query stats.
        /// </summary>
        public KustoQueryStats QueryStats { get; set; }
    }
}
