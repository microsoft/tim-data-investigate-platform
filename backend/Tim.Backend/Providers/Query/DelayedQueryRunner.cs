// <copyright file="DelayedQueryRunner.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Tim.Backend.Providers.Query
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Serilog;
    using Tim.Backend.Models.KustoQuery;
    using Tim.Backend.Providers.Database;

    /// <summary>
    /// Manages executing a long running query and storing the results.
    /// </summary>
    public class DelayedQueryRunner
    {
        private static readonly TimeSpan s_expireTimeSpan = TimeSpan.FromDays(1);
        private static readonly TimeSpan s_immediateResponseWait = TimeSpan.FromSeconds(1);
        private readonly IKustoQueryClient m_queryClient;
        private readonly IDatabaseRepository<KustoQueryRun> m_databaseRepo;
        private readonly ILogger m_logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedQueryRunner"/> class.
        /// </summary>
        /// <param name="queryClient">Kusto client.</param>
        /// <param name="databaseRepo">Database repository.</param>
        public DelayedQueryRunner(IKustoQueryClient queryClient, IDatabaseRepository<KustoQueryRun> databaseRepo)
        {
            m_queryClient = queryClient;
            m_databaseRepo = databaseRepo;
            m_logger = Log.Logger;
        }

        /// <summary>
        /// Creates a long running task to execute the query. If the query runs within a short period, return the results immediately.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<KustoQueryRun> DelayRun(KustoQuery query, CancellationToken cancellationToken)
        {
            var queryRun = new KustoQueryRun(query);

            m_logger.Information($"Adding query run to database {queryRun.QueryRunId}.", "DelayedQueryRunner-DelayRun");
            await m_databaseRepo.AddOrUpdateItemAsync(queryRun, s_expireTimeSpan);

            var runQueryTask = RunAndPersistQuery(queryRun, cancellationToken);
            var timeoutTask = Task.Delay(s_immediateResponseWait, cancellationToken);

            var resultTask = await Task.WhenAny(runQueryTask, timeoutTask).ConfigureAwait(false);

            // If the task completed within the timeout period
            if (resultTask == runQueryTask)
            {
                queryRun = await runQueryTask;
            }

            return queryRun;
        }

        private async Task<KustoQueryRun> RunAndPersistQuery(KustoQueryRun queryRun, CancellationToken cancellationToken)
        {
            try
            {
                m_logger.Information($"Executing kusto query.", "DelayedQueryRunner-RunAndPersistQuery");
                var queryResult = await m_queryClient.RunQuery(queryRun.KustoQuery, cancellationToken);

                queryRun.Status = QueryRunStatus.Completed;
                queryRun.ResultData = queryResult.QueryResults;
                queryRun.ExecutionMetrics = queryResult.QueryStats;
            }
            catch (Exception ex)
            {
                queryRun.Status = QueryRunStatus.Error;
                queryRun.MainError = ex.Message;
                queryRun.StackTrace = ex.StackTrace;
            }

            await m_databaseRepo.AddOrUpdateItemAsync(queryRun);

            return queryRun;
        }
    }
}
