import axios from 'axios';
import auth from '@/helpers/auth';
import runtimeConfig from '@/helpers/runtimeConfig';

export const queryCreate = async (query) => {
  const accessToken = await auth.getApiToken();

  const headers = {
    Authorization: `Bearer ${accessToken}`,
  };

  await axios.post(`${runtimeConfig.apiEndpoint}/templates/queries`, query, {
    headers,
  });
};

export const queryDelete = async (id) => {
  const accessToken = await auth.getApiToken();

  const headers = {
    Authorization: `Bearer ${accessToken}`,
  };

  await axios.delete(`${runtimeConfig.apiEndpoint}/templates/queries/${id}`, {
    headers,
  });
};

export const queryRestore = async (id) => {
  const accessToken = await auth.getApiToken();

  const headers = {
    Authorization: `Bearer ${accessToken}`,
  };

  await axios.patch(
    `${runtimeConfig.apiEndpoint}/templates/queries/${id}`,
    [{ op: 'replace', path: '/isDeleted', value: false }],
    { headers },
  );
};

export const queryRetrieve = async () => {
  const accessToken = await auth.getApiToken();

  const headers = {
    Authorization: `Bearer ${accessToken}`,
  };

  const res = await axios.get(
    `${runtimeConfig.apiEndpoint}/templates/queries?includeDeleted=true`,
    { headers },
  );
  return res.data;
};

export const queryRetrieveById = async (id) => {
  const accessToken = await auth.getApiToken();

  const headers = {
    Authorization: `Bearer ${accessToken}`,
  };

  const res = await axios.get(
    `${runtimeConfig.apiEndpoint}/templates/queries/${id}`,
    { headers },
  );
  return res.data;
};

export const queryUpdate = async (id, query) => {
  const accessToken = await auth.getApiToken();

  const headers = {
    Authorization: `Bearer ${accessToken}`,
  };

  await axios.put(`${runtimeConfig.apiEndpoint}/templates/queries/${id}`, query, {
    headers,
  });
};

export const executeQuery = async (
  cluster,
  database,
  query,
  additionalParameters,
) => {
  const accessToken = await auth.getApiToken();

  const headers = {
    Authorization: `Bearer ${accessToken}`,
  };

  const body = {
    requestedBy: auth.getUserId(),
    cluster,
    database,
    query,
    ...additionalParameters,
  };

  return axios.post(`${runtimeConfig.apiEndpoint}/query/executeQuery`, body, {
    headers,
  });
};

export const getQueryResult = async (queryRunId) => {
  const accessToken = await auth.getApiToken();

  const headers = {
    Authorization: `Bearer ${accessToken}`,
  };

  return axios.get(
    `${runtimeConfig.apiEndpoint}/query/getQueryResult?queryRunId=${queryRunId}`,
    { headers },
  );
};

export const saveEvents = async (events) => {
  const accessToken = await auth.getApiToken();

  const headers = {
    Authorization: `Bearer ${accessToken}`,
  };

  const user = auth.getUserId();
  const dateTimeUtc = new Date().toISOString();

  const data = events.map((event) => ({
    eventId: event.eventId,
    eventTime: event.eventTime,
    createdBy: user,
    dateTimeUtc,
    eventAsJson: event.data,
  }));

  const result = await axios.post(
    `${runtimeConfig.apiEndpoint}/taggedevents/savedEvents`,
    data,
    { headers },
  );

  return result.data
    .filter((e) => e?.success !== true)
    .map((e) => e?.error);
};

export const createTags = async (tags) => {
  const accessToken = await auth.getApiToken();

  const headers = {
    Authorization: `Bearer ${accessToken}`,
  };

  const user = auth.getUserId();
  const dateTimeUtc = new Date().toISOString();

  const data = tags.map((tag) => ({
    eventId: tag.eventId,
    createdBy: user,
    dateTimeUtc,
    tag: tag.tag,
    isDeleted: tag.isDeleted,
  }));

  const result = await axios.post(
    `${runtimeConfig.apiEndpoint}/taggedevents/tags`,
    data,
    { headers },
  );
  return result.data
    .filter((e) => e?.success !== true)
    .map((e) => e?.error);
};

export const createComments = async (comments) => {
  const accessToken = await auth.getApiToken();

  const headers = {
    Authorization: `Bearer ${accessToken}`,
  };

  const user = auth.getUserId();
  const dateTimeUtc = new Date().toISOString();

  const data = comments.map((comment) => ({
    eventId: comment.eventId,
    createdBy: user,
    dateTimeUtc,
    comment: comment.comment,
    determination: comment.determination,
    isDeleted: comment.isDeleted,
  }));

  const result = await axios.post(
    `${runtimeConfig.apiEndpoint}/taggedevents/comments`,
    data,
    { headers },
  );
  return result.data
    .filter((e) => e?.success !== true)
    .map((e) => e?.error);
};

export const createSuppression = async () => 'TODO';
