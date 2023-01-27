import { executeQuery, getQueryResult } from '@/helpers/apiClient';

export const formatCluster = (cluster) => {
  let clusterFormatted = cluster;
  if (!clusterFormatted.trim().endsWith('.kusto.windows.net')) {
    clusterFormatted = `${clusterFormatted}.kusto.windows.net`;
  }
  if (!clusterFormatted.trim().startsWith('https://')) {
    clusterFormatted = `https://${clusterFormatted}`;
  }
  return clusterFormatted;
};

const maxPollTimeout = 11 * 60 * 1000;
const retriesBackOff = 2;
const startBackOff = 500;
const maxBackOff = 30000;
const backOffMultiplier = 2;
export const runKustoQueryPoll = async (
  cluster,
  database,
  query,
  additionalParameters,
) => {
  const startTimer = Date.now();
  let retries = 0;
  let backOffTime = startBackOff;

  const queryRun = await executeQuery(
    formatCluster(cluster),
    database,
    query,
    additionalParameters,
  );

  const { queryRunId } = queryRun.data;

  // TODO: improve this using yield return instead
  while (Date.now() - startTimer < maxPollTimeout) {
    // eslint-disable-next-line no-await-in-loop,no-promise-executor-return,no-loop-func
    await new Promise((resolve) => setTimeout(resolve, backOffTime));

    try {
      // eslint-disable-next-line no-await-in-loop
      const result = await getQueryResult(queryRunId);

      if (result.status === 200) {
        return {
          queryInfo: JSON.parse(result.data.executionMetrics),
          data: result.data?.results || [],
        };
      }
    } catch (err) {
      if (err?.response?.status === 400) {
        throw err?.response?.data?.detail;
      }
      throw err;
    }

    if (retries >= retriesBackOff) {
      retries = 0;
      backOffTime = Math.min(backOffTime * backOffMultiplier, maxBackOff);
    } else {
      retries += 1;
    }
  }
  return null;
};
