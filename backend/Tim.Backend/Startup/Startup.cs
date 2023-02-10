// <copyright file="Startup.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Startup
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;
    using Prometheus;
    using Tim.Backend.Startup.Config;

    /// <summary>
    /// Services start up.
    /// </summary>
    public class Startup
    {
        private const string c_allowAllOriginsPolicy = "allowAllOriginsPolicy";

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">Configurationf for the service.</param>
        /// <param name="environment">Params for enviorment.</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Environment = environment;
            Configuration = configuration;
        }

        /// <summary>
        /// Gets or sets the enviorment the service will run as.
        /// </summary>
        public IWebHostEnvironment Environment { get; set; }

        /// <summary>
        /// Gets the services configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configure services gets called by runtime - it will add services to the running process.
        /// </summary>
        /// <param name="services">Current set up of services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEnvironmentInfo(Configuration);
            services.AddInstrumentationServices(Configuration, Environment.IsDevelopment());
            services.AddConfigurations(Configuration, Environment);
            services.AddMvc();
            services.AddHttpClient();
            services.AddAppServices(Configuration, Environment);
            services.AddControllers()
                .AddNewtonsoftJson(
                opts =>
                {
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                    opts.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });
            services.AddCors(options =>
                {
                    options.AddPolicy(
                        name: c_allowAllOriginsPolicy,
                        builder =>
                        {
                            builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                        });
                });

            services.AddSwagger(Configuration);
            services.AddCouchBase(Configuration);
            services.AddRedis(Configuration);
            services.AddKusto(Configuration);
        }

        /// <summary>
        /// Configure gets called by runtime, it will set up the http pipeline.
        /// </summary>
        /// <param name="app">Specifics for the web app.</param>
        /// <param name="env">Specific for the enviorment http pipeline will run in.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMetricServer();

            app.UseSwagger(Configuration);
            app.UseRouting();
            app.UseHttpMetrics();
            app.UseCors(c_allowAllOriginsPolicy);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });
        }
    }
}
