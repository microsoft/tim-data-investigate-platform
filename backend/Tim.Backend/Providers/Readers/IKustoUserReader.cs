// <copyright file="IKustoUserReader.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Readers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Tim.Backend.DataProviders.Clients;

    /// <summary>
    /// Interface that executes query on behalf of the user.
    /// </summary>
    public interface IKustoUserReader
    {
#pragma warning disable SA1600 // Elements should be documented
        Task<KustoQueryResults<IDictionary<string, object>>> ExecuteQuery(KustoQueryClient kustoClient, string accessToken, string cluster, string db, string query, string queryRunId, string queryId, DateTime startTime, DateTime endTime, CancellationToken ct);
#pragma warning restore SA1600 // Elements should be documented
    }
}
