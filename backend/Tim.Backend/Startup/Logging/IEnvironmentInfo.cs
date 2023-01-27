// <copyright file="IEnvironmentInfo.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Startup.Logging
{
    /// <summary>
    /// Interfase Environment Info.
    /// </summary>
    public interface IEnvironmentInfo
    {
#pragma warning disable SA1600 // Elements should be documented
        string EnvironmentName { get; }

        string DataCenter { get; }

        string Region { get; }

        string AppName { get; }
#pragma warning restore SA1600 // Elements should be documented
    }
}
