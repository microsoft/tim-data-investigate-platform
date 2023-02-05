// <copyright file="IKustoQueryWorker.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Writers.KustoQuery
{
    using System.Threading;
    using System.Threading.Tasks;
    using Tim.Backend.DataProviders.Clients;
    using Tim.Backend.Models;
    using Tim.Backend.Models.KustoQuery;
    using Tim.Backend.Providers.Database;
    using Tim.Backend.Providers.Readers;

    /// <summary>
    /// Class that helps organize everything to get query to be ready to be executed on behalf of user.Interface exists to easily abstract this for local testing without using kusto.
    /// </summary>
    public interface IKustoQueryWorker
    {
#pragma warning disable SA1600 // Elements should be documented
        Task<bool> RunQuery(string token, KustoQueryEventToProcess data, IKustoUserReader customReader, IDatabaseRepository<KustoQueryRun> dbRepository, KustoQueryClient kustoclient, string kustoTableName, CancellationToken cancellationToken);
#pragma warning restore SA1600 // Elements should be documented
    }
}
