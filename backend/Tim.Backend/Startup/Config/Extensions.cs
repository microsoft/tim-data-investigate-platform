// <copyright file="Extensions.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Startup.Config
{
    using System;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Tim.Backend.Startup.Logging;

    /// <summary>
    /// Service extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Add env info to the service.
        /// </summary>
        /// <param name="services">Current services.</param>
        /// <param name="configuration">Current service configuration.</param>
        /// <returns>An updated service object.</returns>
        public static IServiceCollection AddEnvironmentInfo(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddEnvironmentInfo(configuration, nameof(EnvironmentInfo));
        }

        /// <summary>
        /// Add env info to the service.
        /// </summary>
        /// <param name="services">Current services.</param>
        /// <param name="configuration">Current service configuration.</param>
        /// <param name="sectionName">Current section name.</param>
        /// <returns>An updated service object.</returns>
        public static IServiceCollection AddEnvironmentInfo(this IServiceCollection services, IConfiguration configuration, string sectionName)
        {
            var envInfo = new EnvironmentInfo();
            configuration.GetSection(sectionName).Bind(envInfo);
            services.AddSingleton<IEnvironmentInfo>(envInfo);

            return services;
        }
    }

    /// <summary>
    /// Outlined enviorment Info.
    /// </summary>
#pragma warning disable SA1402 // File may only contain a single type
    public class EnvironmentInfo : IEnvironmentInfo
#pragma warning restore SA1402 // File may only contain a single type
    {
        /// <summary>
        /// Gets or sets the service enviorment name.
        /// </summary>
        public string EnvironmentName { get; set; } = Environment.GetEnvironmentVariable("ENVIRONMENT");

        /// <summary>
        /// Gets or sets the service data center.
        /// </summary>
        public string DataCenter { get; set; } = Environment.GetEnvironmentVariable("DATA_CENTER");

        /// <summary>
        /// Gets or sets the service region.
        /// </summary>
        public string Region { get; set; } = Environment.GetEnvironmentVariable("REGION");

        /// <summary>
        /// Gets or sets the service app name.
        /// </summary>
        public string AppName { get; set; } = Environment.GetEnvironmentVariable("APP_NAME");
    }
}
