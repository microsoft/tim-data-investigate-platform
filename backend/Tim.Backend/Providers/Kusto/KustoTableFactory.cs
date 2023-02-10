// <copyright file="KustoTableFactory.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Kusto
{
    using Tim.Backend.Models.TaggedEvents.Tables;

    /// <summary>
    /// Factory for kusto tables.
    /// </summary>
    public class KustoTableFactory
    {
        /// <summary>
        /// Create a new table specification.
        /// </summary>
        /// <typeparam name="T">Kusto table type to create.</typeparam>
        /// <param name="database">Database name.</param>
        /// <returns>Kusto table specification.</returns>
        public static T CreateKustoTableSpec<T>(string database)
            where T : IKustoTable, new()
        {
            return new T()
            {
                DatabaseName = database,
            };
        }
    }
}
