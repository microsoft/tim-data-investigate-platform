// <copyright file="ServiceWebHostSetup.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Tests.Setup
{
    using Microsoft.AspNetCore.Hosting;
    using System;
    using Tim.Flow;

    public sealed class ServiceWebHostSetup
    {
        private static string EnvironmentName => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        private static readonly object SetupLock = new();
        private static IWebHost s_serviceWebHost;

        public static IWebHost Initialize()
        {
            lock (SetupLock)
            {
                if (s_serviceWebHost == null)
                {
                    Env.SetEnvironment(RuntimeEnvironment.PPE);

                    s_serviceWebHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder()
                                    .ConfigureAppConfiguration((config) =>
                                    {

                                    })
                                    .UseStartup<TestServerStartup>()
                                    .Build();
                }

                return s_serviceWebHost;
            }
        }
    }
}
