// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace TIM.Backend
{
    using System;

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Tim.Backend.Startup;
    using Tim.Backend.Startup.Config;
    using Tim.Flow;

    /// <summary>
    /// Program class that starts the backend service.
    /// </summary>
    public sealed class Program
    {
        private static string EnvironmentName => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">Command line args.</param>
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        /// <summary>
        /// Builds out the web host for the service.
        /// </summary>
        /// <param name="args">Command line args.</param>
        /// <returns>Web host.</returns>
        public static IWebHost BuildWebHost(string[] args)
        {
            Env.SetEnvironment(EnvironmentName == "Production" ? RuntimeEnvironment.PROD : RuntimeEnvironment.PPE);

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((config) =>
                {
                    config.Build();

                    if (Env.CurrentEnvironment == RuntimeEnvironment.PPE)
                    {
                        config.AddUserSecrets<AzureResourcesSectionSecrets>();
                    }
                })
                .UseStartup<Startup>()
                .Build();
        }
    }
}
