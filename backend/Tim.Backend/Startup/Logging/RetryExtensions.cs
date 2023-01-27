// <copyright file="RetryExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Startup.Logging
{
    using Microsoft.Extensions.Logging;

    using Polly;

    /// <summary>
    /// Service retry extensions.
    /// </summary>
    public static class RetryExtensions
    {
        /// <summary>
        /// Basic retry policy name const.
        /// </summary>
        internal const string BasicRetryPolicyName = "__BasicRetryPolicy__";

        /// <summary>
        /// Basic async retry policy name const.
        /// </summary>
        internal const string BasicAsyncRetryPolicyName = "__BasicAsyncRetryPolicy__";

        /// <summary>
        /// Logger const.
        /// </summary>
        internal const string Logger = "__Logger__";

        /// <summary>
        /// Gets a logger, if present from a <see cref="Context"/>.
        /// </summary>
        /// <param name="context">The context to check.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>True if logger was retrieved, false it is null.</returns>
        public static bool TryGetLogger(this Context context, out ILogger logger)
        {
            if (context.TryGetValue(Logger, out var loggerObject) && loggerObject is ILogger theLogger)
            {
                logger = theLogger;
                return true;
            }

            logger = null;
            return false;
        }
    }
}
