import { queryRetrieve } from '@/helpers/apiClient';
import { QueryTemplate } from '@/helpers/kustoQueries';
import Vue from 'vue';
import localforage from 'localforage';

const queryOptionStore = localforage.createInstance({
  driver: localforage.INDEXEDDB,
  storeName: 'query_options',
});

const getters = {
  getQueryTemplates: (state) => state.templates.filter((query) => query.queryType === 'query'),
  getViewTemplates: (state) => state.templates
    .filter((query) => query.queryType === 'view')
    .sort((a, b) => a.menu.localeCompare(b.menu)),
  getTemplate: (state) => (uuid) => state.templates.find((query) => query.uuid === uuid),
  getQueryOption: (state) => (uuid) => state.queryOptions[uuid] ?? {},
};

const actions = {
  async getQueries({ commit, state }, reload = false) {
    if (!state.queryPromise || reload) {
      commit('setQueryPromise', queryRetrieve());
    }

    return state.queryPromise;
  },
  async loadQueries({ commit, dispatch, rootGetters }, reload = false) {
    const queries = await dispatch('getQueries', reload);
    await dispatch('functions/loadFunctions', null, { root: true });

    const templates = queries
      .filter((query) => query.isDeleted !== true)
      .map((query) => {
        const functions = (query.functions ?? [])
          .map((uuid) => rootGetters['functions/getFunctionByUuid'](uuid));
        return new QueryTemplate({ ...query, functions });
      });

    const queryOptions = {};
    await queryOptionStore.iterate((queryOption, uuid) => {
      queryOptions[uuid] = queryOption;
    });

    commit('setTemplates', templates);
    commit('setQueryOptions', queryOptions);
  },
  async reloadQueries({ dispatch }) {
    await dispatch('loadQueries', true);
  },
  addQuery({ commit, state }, query) {
    const templates = [...state.templates, new QueryTemplate(query)];
    commit('setTemplates', templates);
  },
  async updateQueryOption({ commit, state }, { uuid, queryOption }) {
    const prevQueryOption = state.queryOptions[uuid] ?? {};
    const newQueryOption = { ...prevQueryOption, ...queryOption };
    await queryOptionStore.setItem(uuid, newQueryOption);
    commit('setQueryOption', { uuid, queryOption: newQueryOption });
  },
  updateQuery({ commit, state }, query) {
    const templates = [
      ...state.templates.filter((q) => q.uuid !== query.uuid),
      new QueryTemplate(query),
    ];
    commit('setTemplates', templates);
  },
  removeQuery({ commit, state }, uuid) {
    const templates = [...state.templates.filter((q) => q.uuid !== uuid)];
    commit('setTemplates', templates);
  },
};

const mutations = {
  setTemplates(state, templates) {
    state.templates = templates;
  },
  setQueryPromise(state, queryPromise) {
    state.queryPromise = queryPromise;
  },
  setQueryOptions(state, queryOptions) {
    state.queryOptions = queryOptions;
  },
  setQueryOption(state, { uuid, queryOption }) {
    Vue.set(state.queryOptions, uuid, queryOption);
  },
};

const state = () => ({
  templates: [],
  queryOptions: {},
  queryPromise: null,
});

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations,
};
