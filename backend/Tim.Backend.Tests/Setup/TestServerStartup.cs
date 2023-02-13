// <copyright file="TestServerStartup.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Tests.Setup
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Tim.Backend.Startup.Config;

    /// <summary>
    /// Initializes the services and configuration needed for the test cases.
    /// </summary>
    public class TestServerStartup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestServerStartup"/> class.
        /// </summary>
        /// <param name="configuration">DI configuration.</param>
        /// <param name="environment">DI environment.</param>
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
        }
    }
}
