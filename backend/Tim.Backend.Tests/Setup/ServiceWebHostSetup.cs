// <copyright file="ServiceWebHostSetup.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Tests.Setup
{
    using Microsoft.AspNetCore.Hosting;

    public sealed class ServiceWebHostSetup
    {
        private static readonly object SetupLock = new();
        private static IWebHost s_serviceWebHost;

        public static IWebHost Initialize()
        {
            lock (SetupLock)
            {
                if (s_serviceWebHost == null)
                {
                    s_serviceWebHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder()
                        .ConfigureAppConfiguration((config) => { })
                        .UseStartup<TestServerStartup>()
                        .Build();
                }

                return s_serviceWebHost;
            }
        }
    }
}
