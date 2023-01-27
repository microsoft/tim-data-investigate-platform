// <copyright file="TestServerStartup.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Tests.Setup
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Tim.Backend.Providers.Readers;
    using Tim.Backend.Startup;
    using Tim.Backend.Startup.Config;
    using Tim.Common;

    /// <summary>
    /// Initializes the services and configuration needed for the test cases.
    /// </summary>
    public class TestServerStartup
    {
        private const string AzureADSectionName = "AzureAd";

        public TestServerStartup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Environment = environment;
            Configuration = configuration;
        }

        /// <summary>
        /// Gets or Sets env that service is running under.
        /// </summary>
        public IWebHostEnvironment Environment { get; set; }

        /// <summary>
        /// Gets the current service configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">provides the collection if service that are running.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Calling the AddConfigurations() extensions from the main service
            // Bind the configuration using IOptions to your concrete settings class
            services.Configure<KustoConfiguration>(Configuration.GetSection(nameof(KustoConfiguration)));

            services.AddScoped<IKustoUserReader, KustoUserReader>();

            services.AddSingleton<ISharedCache, InMemoryCache>();
        }

        public void Configure()
        {
        }
    }
}
