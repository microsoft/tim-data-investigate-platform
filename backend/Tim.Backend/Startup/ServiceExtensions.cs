// <copyright file="ServiceExtensions.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Startup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Kusto.Cloud.Platform.Utils;
    using Kusto.Data;
    using Kusto.Data.Net.Client;
    using Kusto.Ingest;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Identity.Client;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using Serilog;
    using StackExchange.Redis;
    using Tim.Backend.Models.KustoQuery;
    using Tim.Backend.Models.TaggedEvents.Tables;
    using Tim.Backend.Models.Templates;
    using Tim.Backend.Providers.Database;
    using Tim.Backend.Providers.Kusto;
    using Tim.Backend.Startup;
    using Tim.Backend.Startup.Config;

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
            var authConfigs = configuration.GetSection(nameof(AuthConfiguration)).Get<AuthConfiguration>() ?? new AuthConfiguration();
            authConfigs.Validate();

            var authenticationSchemes = new List<string>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Audience = authConfigs.ClientId;
                    options.Authority = $"https://login.microsoftonline.com/{authConfigs.ClientAuthority}";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "https://login.microsoftonline.com/{authConfigs.ClientAuthority}",
                        ValidAudience = $"api://{authConfigs.ClientId}",
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        RequireSignedTokens = true,
                    };
                    options.SaveToken = true;
                });

            services.AddSingleton(p =>
            {
                return ConfidentialClientApplicationBuilder.Create(authConfigs.ClientId)
                    .WithAuthority($"https://login.microsoftonline.com/{authConfigs.ClientAuthority}")
                    .WithClientSecret(authConfigs.ClientSecret)
                    .Build();
            });

            services.AddPolicyRegistry();
        }

        /// <summary>
        /// Initialization code for services.
        /// </summary>
        /// <param name="host">The host object to modify.</param>
        /// <exception cref="ArgumentNullException">Error if any expected configurations are null.</exception>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task InitializeAsync(this IWebHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            await host.InitializeDatabaseAsync();
            await host.InitializeKustoAsync();
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
            await dbService.ConnectAsync();
            await dbService.InitializeAsync();
        }

        /// <summary>
        /// Initialization code for the kusto tables.
        /// </summary>
        /// <param name="host">The host object to modify.</param>
        /// <exception cref="ArgumentNullException">Error if any expected configurations are null.</exception>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task InitializeKustoAsync(this IWebHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            var scope = host.Services.CreateScope();
            var adminClient = scope.ServiceProvider.GetService<KustoAdminClient>();

            var savedEventTable = scope.ServiceProvider.GetService<SavedEventTable>();
            var eventCommentTable = scope.ServiceProvider.GetService<EventCommentTable>();
            var eventTagTable = scope.ServiceProvider.GetService<EventTagTable>();

            await adminClient.CreateTableAsync(savedEventTable);
            await adminClient.CreateTableAsync(eventCommentTable);
            await adminClient.CreateTableAsync(eventTagTable);

            await adminClient.CreateTableMappingAsync(savedEventTable);
            await adminClient.CreateTableMappingAsync(eventCommentTable);
            await adminClient.CreateTableMappingAsync(eventTagTable);
        }

        /// <summary>
        /// Add couchbase service.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Defined configuration.</param>
        /// <exception cref="ArgumentNullException">Error if any expected configurations are null.</exception>
        public static void AddCouchBase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var dbConfigs = configuration.GetSection(nameof(DatabaseConfiguration)).Get<DatabaseConfiguration>() ?? new DatabaseConfiguration();
            dbConfigs.Validate();

            var cbClient = new CouchbaseDbClient(dbConfigs);
            services.AddScoped<IDatabaseClient, CouchbaseDbClient>(p => cbClient);
            services.AddScoped<IDatabaseRepository<KustoQueryRun>, CouchbaseRepository<KustoQueryRun>>(p => new CouchbaseRepository<KustoQueryRun>(cbClient));
            services.AddScoped<IDatabaseRepository<QueryTemplate>, CouchbaseRepository<QueryTemplate>>(p => new CouchbaseRepository<QueryTemplate>(cbClient));
        }

        /// <summary>
        /// Add redis service.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Defined configuration.</param>
        /// <exception cref="ArgumentNullException">Error if any expected configurations are null.</exception>
        public static void AddRedis(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var redisConfigs = configuration.GetSection(nameof(RedisConfiguration)).Get<RedisConfiguration>() ?? new RedisConfiguration();
            redisConfigs.Validate();

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
        }

        /// <summary>
        /// Add Kusto service.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        /// <param name="configuration">Defined configuration.</param>
        /// <exception cref="ArgumentNullException">Error if any expected configurations are null.</exception>
        public static void AddKusto(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var kustoConfigs = configuration.GetSection(nameof(KustoConfiguration)).Get<KustoConfiguration>() ?? new KustoConfiguration();
            kustoConfigs.Validate();

            var connectionString = new KustoConnectionStringBuilder(kustoConfigs.KustoClusterUri, kustoConfigs.KustoDatabase)
                .WithAadApplicationKeyAuthentication(kustoConfigs.KustoAppId, kustoConfigs.KustoAppKey, kustoConfigs.KustoAppAuthority);

            services.AddScoped(p =>
            {
                var client = KustoIngestFactory.CreateDirectIngestClient(connectionString);
                return new KustoIngestClient(client);
            });

            services.AddScoped(p =>
            {
                var client = KustoClientFactory.CreateCslAdminProvider(connectionString);
                return new KustoAdminClient(client);
            });

            services.AddScoped(p => KustoTableFactory.CreateKustoTableSpec<SavedEventTable>(kustoConfigs.KustoDatabase));
            services.AddScoped(p => KustoTableFactory.CreateKustoTableSpec<EventTagTable>(kustoConfigs.KustoDatabase));
            services.AddScoped(p => KustoTableFactory.CreateKustoTableSpec<EventCommentTable>(kustoConfigs.KustoDatabase));
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
                c.RouteTemplate = serverConfigs.ApiBasePath + "swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/{serverConfigs.ApiBasePath}swagger/v1/swagger.json", "API");
                c.RoutePrefix = $"{serverConfigs.ApiBasePath}swagger";
            });
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
