import store from '@/store';
import eventBus from '@/helpers/eventBus';
import router from '@/router';
import { deleteDataStore, saveDataStore } from '@/helpers/localStorage';
import { runKustoQueryPoll } from '@/helpers/queries';

export const convertToCustomQuery = async (uuid, queryTemplate, params) => {
  const cluster = queryTemplate.buildCluster(params);
  const { database } = queryTemplate;
  const query = queryTemplate.buildQuery(params);

  await store.dispatch('displayComponent/convertDisplayComponent', {
    uuid,
    componentName: 'KustoQueryResult',
    params: {
      query,
      cluster,
      database,
    },
    state: {
      isVisited: false,
      error: null,
      rowCount: null,
      isExecuting: false,
    },
  });
};

export const defaultNewQuery = () => `declare query_parameters(StartTime:datetime, EndTime:datetime);
DeviceProcessEvents
| where Timestamp between (StartTime .. EndTime)
| take 1
| extend EventTime=Timestamp, Cluster=current_cluster_endpoint(), EventId=strcat(DeviceId, ReportIndex)`;

export const createNewQueryComponent = async (
  title,
  query = defaultNewQuery(),
  cluster = '',
  database = '',
) => {
  const uuid = await store.dispatch('displayComponent/createDisplayComponent', {
    componentName: 'KustoQueryResult',
    title,
    params: {
      query,
      cluster,
      database,
    },
    parentUuid: null,
    state: {
      isVisited: false,
      error: null,
      rowCount: null,
      isExecuting: false,
    },
    rowDataTrigger: null,
  });
  await router.push({ name: 'OpenTriage', params: { uuid } });
};

export const runNewQuery = async (
  uuid,
  query,
  cluster,
  database,
  additionalParameters = {},
) => {
  await store.dispatch('displayComponent/updateComponentState', {
    uuid,
    isExecuting: true,
    isVisited: false,
    error: null,
  });

  try {
    const result = await runKustoQueryPoll(
      cluster,
      database,
      query,
      additionalParameters,
    );

    await store.dispatch('displayComponent/updateComponentState', {
      uuid,
      isExecuting: false,
      executionTime: result.queryInfo.execution_time,
      cpuUsage: result.queryInfo?.resource_usage?.cpu['total cpu'],
      memoryUsage: result.queryInfo?.resource_usage?.memory?.peak_per_node,
      rowCount: result.data.length,
    });

    eventBus.$emit('update:kusto-results', { uuid });

    if (result.data.length === 0) {
      await saveDataStore(uuid, []);
      await store.dispatch('displayComponent/triggerComponentRowData', uuid);
      console.info('No results found...');
      return;
    }

    await saveDataStore(uuid, result.data);
    await store.dispatch('displayComponent/triggerComponentRowData', uuid);
  } catch (err) {
    await store.dispatch('displayComponent/updateComponentState', {
      uuid,
      error: err,
      isExecuting: false,
    });

    await deleteDataStore(uuid);
    await store.dispatch('displayComponent/triggerComponentRowData', uuid);
  }
};

export const runTemplateQuery = async (uuid) => {
  const { queryTemplate, inParams } = store.getters['displayComponent/getComponentParams'](uuid);

  const cluster = queryTemplate.buildCluster(inParams);
  const { database } = queryTemplate;
  const query = queryTemplate.buildQuery(inParams);

  await runNewQuery(uuid, query, cluster, database);
};

export const templateQueriesAsObject = (queries) => {
  const lookup = {};
  const result = [];
  queries.forEach((query) => {
    let currentList = result;
    query.path.forEach((path) => {
      if (!(path in lookup)) {
        lookup[path] = {
          title: path,
          items: [],
        };
        currentList.push(lookup[path]);
      }
      currentList = lookup[path].items;
    });
    currentList.push(query);
  });
  return result;
};

export const createNewTemplateQueryComponent = async (
  title,
  queryTemplate,
  params,
  parentUuid,
  autoExecuteQuery = false,
  makeActive = false,
) => {
  // Deep clone the params (support for objects/arrays)
  const cloneParams = JSON.parse(JSON.stringify(params));

  const isDataComplete = queryTemplate.isDataComplete(cloneParams);

  const uuid = await store.dispatch('displayComponent/createDisplayComponent', {
    componentName: 'TemplateQueryResult',
    title,
    params: {
      inParams: cloneParams,
      queryTemplate,
    },
    parentUuid,
    state: {
      isVisited: false,
      error: null,
      rowCount: null,
      isExecuting: false,
      editQuery: !autoExecuteQuery || !isDataComplete,
    },
    rowDataTrigger: null,
  });

  if (autoExecuteQuery && isDataComplete) {
    await runTemplateQuery(uuid);
  }

  if (makeActive || !isDataComplete) {
    await router.push({ name: 'OpenTriage', params: { uuid } });
  }
};
