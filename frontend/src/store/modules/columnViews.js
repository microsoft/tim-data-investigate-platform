import { generateUuidv4 } from '@/helpers/utils';
import localforage from 'localforage';
import Vue from 'vue';

const columnViewStore = localforage.createInstance({
  driver: localforage.INDEXEDDB,
  storeName: 'column_views',
});

const getters = {
  getColumnView: (state) => (uuid) => state.columnViews[uuid],
  getAllColumnViews: (state) => Object.values(state.columnViews)
    .sort((a, b) => (a.name > b.name ? 1 : -1)),
};

const actions = {
  async addColumnView({ commit }, { name, columnState }) {
    const uuid = generateUuidv4();

    const columnView = {
      uuid,
      name,
      columnState,
    };

    await columnViewStore.setItem(uuid, columnView);
    commit('setColumnView', columnView);

    return uuid;
  },
  async updateColumnView({ commit, state }, { uuid, ...columnView }) {
    const newColumnView = {
      ...state.columnViews[uuid],
      ...columnView,
    };

    await columnViewStore.setItem(uuid, newColumnView);
    commit('setColumnView', newColumnView);
  },
  async removeColumnView({ commit }, uuid) {
    await columnViewStore.removeItem(uuid);
    commit('removeColumnView', uuid);
  },
  async loadAllColumnViews({ commit }) {
    const columnViews = {};
    await columnViewStore.iterate((columnView) => {
      columnViews[columnView.uuid] = columnView;
    });

    commit('setColumnViews', columnViews);
  },
};

const mutations = {
  setColumnView(state, columnView) {
    Vue.set(state.columnViews, columnView.uuid, columnView);
  },
  setColumnViews(state, columnViews) {
    state.columnViews = columnViews;
  },
  removeColumnView(state, uuid) {
    Vue.delete(state.columnViews, uuid);
  },
};

const state = () => ({
  columnViews: {},
});

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations,
};
