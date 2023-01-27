// <copyright file="RedisKeyGenerator.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Helpers
{
    using System;

    /// <summary>
    /// Redis Key generator, used for name creation of data that gets stored in redis.
    /// </summary>
    public class RedisKeyGenerator
    {
        /// <summary>
        /// Generates a unique value for where query run results will be stored.
        /// </summary>
        /// <param name="queryRunId">Unique query run id.</param>
        /// <returns>Generated path for the unique query run.</returns>
        public static string QueryRun(Guid queryRunId)
        {
            return $"/queryResults/{queryRunId}";
        }
    }
}
