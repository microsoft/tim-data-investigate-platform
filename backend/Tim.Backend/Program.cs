// <copyright file="Program.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace TIM.Backend
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Tim.Backend.Startup;
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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            var host = BuildWebHost(args);
            await host.InitializeAsync();
            await host.RunAsync();
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
                })
                .UseStartup<Startup>()
                .Build();
        }
    }
}
