// <copyright file="ServiceCollectionExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Startup
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Polly;
    using Polly.Registry;
    using Tim.Backend.Startup.Logging;

    /// <summary>
    /// Services Collection Extensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add a policy registry, with some basic retry helpers, to the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="basicRetryNumberOfRetries">For the basic retry helper, how many retries?.</param>
        /// <param name="basicRetryTimeBetweenRetries">For the basic retry helper, how long between retries?.</param>
        /// <param name="policies">Other policies to add to the registry?.</param>
        /// <returns>An updated Services collection.</returns>
        public static IServiceCollection AddPolicyRegistry(
            this IServiceCollection serviceCollection,
            int basicRetryNumberOfRetries = 3,
            TimeSpan basicRetryTimeBetweenRetries = default,
            IDictionary<string, IsPolicy> policies = null)
        {
            var registry = new PolicyRegistry();

            var retryTime = basicRetryTimeBetweenRetries;
            if (retryTime == default)
            {
                retryTime = TimeSpan.FromSeconds(5);
            }

            registry[RetryExtensions.BasicRetryPolicyName] =
                Policy
                    .Handle<Exception>()
                    .WaitAndRetry(
                    retryCount: basicRetryNumberOfRetries,
                    (_) => retryTime,
                    (e, ts, rc, ctx) =>
                    {
                        if (!ctx.TryGetLogger(out var logger) || e == null)
                        {
                            return;
                        }

                        logger.LogWarning(
                            $"Failed to run {{operation}} on try {{executionCount}} of 3 with exception {{exception}}. Waiting {ts}.",
                            ctx.OperationKey,
                            rc,
                            e);
                    });

            registry[RetryExtensions.BasicAsyncRetryPolicyName] =
                Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(
                    retryCount: basicRetryNumberOfRetries,
                    (_) => basicRetryTimeBetweenRetries,
                    (e, ts, rc, ctx) =>
                    {
                        if (!ctx.TryGetLogger(out var logger) || e == null)
                        {
                            return;
                        }

                        logger.LogWarning(
                            $"Failed to run {{operation}} on try {{executionCount}} of 3 with exception {{exception}}. Waiting {ts}.",
                            ctx.OperationKey,
                            rc,
                            e);
                    });

            if (policies != null)
            {
                foreach (var policy in policies)
                {
                    registry.Add(policy.Key, policy.Value);
                }
            }

            serviceCollection.AddSingleton<IReadOnlyPolicyRegistry<string>>(registry);

            return serviceCollection;
        }
    }
}
