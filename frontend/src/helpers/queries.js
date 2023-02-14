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

const handleResult = (result) => {
  if (result.status !== 200) {
    throw new Error(`Unexpected status code ${result.status}.`);
  }

  if (result.data?.status === 'completed') {
    return {
      queryInfo: result.data.executionMetrics,
      data: result.data?.resultData || [],
    };
  }

  if (result.data?.status === 'error') {
    throw result?.data?.mainError;
  }

  throw new Error(`Unexpected status ${result.data?.status}`);
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

  let queryRunId;

  try {
    // eslint-disable-next-line no-await-in-loop
    const result = await executeQuery(
      formatCluster(cluster),
      database,
      query,
      additionalParameters,
    );

    if (result.status === 202) {
      queryRunId = result.data?.queryRunId;
    } else if (result.status === 200) {
      return handleResult(result);
    }
  } catch (err) {
    if (err?.response?.status === 400) {
      throw err?.response?.data?.detail;
    }
    throw err;
  }

  if (queryRunId === null) {
    throw Error('queryRunId missing from response.');
  }

  // TODO: improve this using yield return instead
  while (Date.now() - startTimer < maxPollTimeout) {
    // eslint-disable-next-line no-await-in-loop,no-promise-executor-return,no-loop-func
    await new Promise((resolve) => setTimeout(resolve, backOffTime));

    try {
      // eslint-disable-next-line no-await-in-loop
      const result = await getQueryResult(queryRunId);

      if (result.status === 200) {
        return handleResult(result);
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
