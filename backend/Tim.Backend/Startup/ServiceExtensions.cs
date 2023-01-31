// <copyright file="ServiceExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Startup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using Kusto.Cloud.Platform.Utils;
    using Kusto.Data;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using Serilog;
    using StackExchange.Redis;
    using Tim.Backend.DataProviders.Clients;
    using Tim.Backend.Providers.Database;
    using Tim.Backend.Providers.Readers;
    using Tim.Backend.Providers.Writers.KustoQuery;
    using Tim.Backend.Startup;
    using Tim.Backend.Startup.Config;
    using Tim.Common;

    /// <summary>
    /// Defines all the service extensions.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Add proper service configurations.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Defined configuration.</param>
        /// <param name="environment">Enviorment service is running in.</param>
        /// <exception cref="ArgumentNullException">Error if any expected configurations are null.</exception>
        public static void AddConfigurations(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (environment is null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            services.AddOptions();
            services.Configure<KustoConfiguration>(configuration.GetSection(nameof(KustoConfiguration)));
            services.Configure<DatabaseConfiguration>(configuration.GetSection(nameof(DatabaseConfiguration)));
            services.Configure<AuthConfiguration>(configuration.GetSection(nameof(AuthConfiguration)));
            services.Configure<RedisConfiguration>(configuration.GetSection(nameof(RedisConfiguration)));
            services.Configure<SwaggerConfiguration>(configuration.GetSection(nameof(SwaggerConfiguration)));
        }

        /// <summary>
        /// Add logging services.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Defined configuration.</param>
        /// <param name="isDevelopment">Enviorment specification.</param>
        public static void AddInstrumentationServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            if (isDevelopment)
            {
                services.AddLogging(builder =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .WriteTo.Console()
                        .WriteTo.File(
                            "log.txt",
                            rollingInterval: RollingInterval.Hour,
                            rollOnFileSizeLimit: true)
                        .CreateLogger();

                    builder.AddSerilog(Log.Logger);
                });
            }
            else
            {
                services.AddLogging(builder =>
                {
                    Log.Logger = new LoggerConfiguration()
                                       .MinimumLevel.Information()
                                       .WriteTo.Console()
                                       .CreateLogger();

                    builder.AddSerilog(Log.Logger);
                });
            }
        }

        /// <summary>
        /// Add all services worker and object instances needed for those to run.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Defined configuration.</param>
        /// <param name="environment">Enviorment service is running in.</param>
        /// <exception cref="ArgumentNullException">Error if any expected configurations are null.</exception>
        public static void AddAppServices(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (environment is null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            // create  kusto and database configuration since it is not being loaded from services set up, but grabbing either env or default values
            var redisConfigs = configuration.GetSection(nameof(RedisConfiguration)).Get<RedisConfiguration>() ?? new RedisConfiguration();
            var kustoConfigs = configuration.GetSection(nameof(KustoConfiguration)).Get<KustoConfiguration>() ?? new KustoConfiguration();
            var authConfigs = configuration.GetSection(nameof(AuthConfiguration)).Get<AuthConfiguration>() ?? new AuthConfiguration();

            redisConfigs.Validate();
            kustoConfigs.Validate();
            authConfigs.Validate();

            services.AddSingleton(p =>
            {
                var connectionString = new KustoConnectionStringBuilder
                {
                    DataSource = kustoConfigs.IngestKustoClusterUri,
                    InitialCatalog = kustoConfigs.KustoDatabase,
                    FederatedSecurity = true,
                    ApplicationClientId = kustoConfigs.KustoAppId,
                    ApplicationKey = kustoConfigs.KustoAppKey,
                    ApplicationNameForTracing = "TIM-Ingest-To-Kusto",
                };

                return new KustoIngestClient(connectionString);
            });

            services.AddSingleton(p =>
            {
                var connectionString = new KustoConnectionStringBuilder
                {
                    DataSource = kustoConfigs.KustoClusterUri,
                    InitialCatalog = kustoConfigs.KustoDatabase,
                    FederatedSecurity = true,
                    ApplicationClientId = kustoConfigs.KustoAppId,
                    ApplicationKey = kustoConfigs.KustoAppKey,
                    ApplicationNameForTracing = "TIM-Query-To-Kusto",
                };

                return new KustoQueryClient(connectionString);
            });

            var authenticationSchemes = new List<string>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfigs.SigningKey)),
                    };
                });

            services.AddPolicyRegistry();

            services.AddSingleton<IKustoQueryWorker, RunKustoQueryWorker>();

            services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(p =>
            {
                var endpoints = new EndPointCollection();
                redisConfigs.RedisHosts.Split(",").ForEach(host => endpoints.Add(host));
                return ConnectionMultiplexer.Connect(
                    new ConfigurationOptions()
                    {
                        EndPoints = endpoints,
                        Password = redisConfigs.RedisPassword,
                    });
            });

            var dbConfigs = configuration.GetSection(nameof(DatabaseConfiguration)).Get<DatabaseConfiguration>() ?? new DatabaseConfiguration();
            dbConfigs.Validate();

            services.AddScoped<IDatabaseClient, CouchbaseDbClient>(p => new CouchbaseDbClient(dbConfigs));

            services.AddScoped<IKustoUserReader, KustoUserReader>();

            services.AddSingleton<ISharedCache, InMemoryCache>();
        }

        /// <summary>
        /// Initialization code for the database client.
        /// </summary>
        /// <param name="host">The host object to modify.</param>
        /// <exception cref="ArgumentNullException">Error if any expected configurations are null.</exception>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task InitializeDatabaseAsync(this IWebHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            var scope = host.Services.CreateScope();
            var dbService = scope.ServiceProvider.GetService<IDatabaseClient>();
            await dbService.ConnectDatabase();
        }

        /// <summary>
        /// Add swagger configuration.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Defined configuration.</param>
        /// <exception cref="ArgumentNullException">Error if any expected configurations are null.</exception>
        public static void AddSwagger(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "TIM.Backend",
                        Version = "v1",
                    });

                var xmldocPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                if (File.Exists(xmldocPath))
                {
                    c.IncludeXmlComments(xmldocPath);
                }

                // Schema IDs are by default Type's Name but unfortunately we have collisions.
                c.CustomSchemaIds(t => t.FullName);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token e.g. \"eyJh...\"",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        new string[] { }
                    },
                });
            });
        }

        /// <summary>
        /// Set swagger configurations in http.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="configuration">Defined configuration.</param>
        /// <exception cref="ArgumentNullException">Error if any expected configurations are null.</exception>
        public static void UseSwagger(
            this IApplicationBuilder app,
            IConfiguration configuration)
        {
            if (app is null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var serverConfigs = configuration.GetSection(nameof(SwaggerConfiguration)).Get<SwaggerConfiguration>() ?? new SwaggerConfiguration();
            serverConfigs.Validate();

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    // Expose swagger on the request's originating port/host
                    //  NOTE: in K8s, our Nginx ingress controller adds these headers
                    var protocol = httpReq.Headers.TryGetHeaderValueWithDefault("X-Forwarded-Proto", httpReq.Scheme);
                    var hostName = httpReq.Headers.TryGetHeaderValueWithDefault("X-Forwarded-Host", httpReq.Host.ToString());
                    var serverUrl = $"{protocol}://{hostName}{serverConfigs.ApiBasePath}";

                    // Servers property on swagger is required for certain scanners to work correctly.
                    swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = serverUrl } };
                });
            });

            app.UseSwaggerUI();
        }

        private static string TryGetHeaderValueWithDefault(
            this IHeaderDictionary headers,
            string headerName,
            string defaultValue)
        {
            return headers.TryGetValue(headerName, out var headerValue) && !string.IsNullOrEmpty(headerValue)
                ? headerValue.ToString()
                : defaultValue;
        }
    }
}
