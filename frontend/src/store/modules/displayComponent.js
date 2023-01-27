import { generateUuidv4 } from '@/helpers/utils';
import eventBus from '@/helpers/eventBus';
import { QueryTemplate } from '@/helpers/kustoQueries';
import localforage from 'localforage';
import { deleteDataStore } from '@/helpers/localStorage';

const componentStore = localforage.createInstance({
  driver: localforage.INDEXEDDB,
  storeName: 'display_components',
});

const getters = {
  isComponent: (state) => (uuid) => uuid in state.displayComponents,
  getComponentParams: (state) => (uuid) => state.displayComponents[uuid].params,
  getComponentTitle: (state) => (uuid) => state.displayComponents[uuid].title,
  getComponentRowDataTrigger: (state) => (uuid) => state.displayComponents[uuid].rowDataTrigger,
  getComponentParentUuid: (state) => (uuid) => state.displayComponents[uuid].parentUuid,
  getComponentName: (state) => (uuid) => state.displayComponents[uuid].componentName,
  getComponentState: (state) => (uuid) => state.displayComponents[uuid].state,
  getRootDisplayComponents: (state) => state.rootDisplayComponents,
  getParentDisplayComponent: (state) => (uuid) => state.displayComponents[uuid].parentUuid,
};

const actions = {
  removeDisplayComponent({ commit, dispatch }, { uuid }) {
    commit('removeDisplayComponent', uuid);
    dispatch('removeDisplayComponentPersist', uuid);
  },
  removeAllDisplayComponents({ commit, dispatch }, { uuidArray }) {
    uuidArray.forEach((uuid) => {
      commit('removeDisplayComponent', uuid);
      dispatch('removeDisplayComponentPersist', uuid);
    });
  },
  updateComponentState({ commit, state, dispatch }, { uuid, ...newState }) {
    const updateState = {
      ...state.displayComponents[uuid].state,
      ...newState,
    };
    commit('setComponentState', {
      uuid,
      newState: updateState,
    });
    dispatch('saveDisplayComponent', uuid);
  },
  updateComponentParams({ commit, state, dispatch }, { uuid, ...newParams }) {
    const updateParams = {
      ...state.displayComponents[uuid].params,
      ...newParams,
    };
    commit('setComponentParams', {
      uuid,
      newParams: updateParams,
    });
    dispatch('saveDisplayComponent', uuid);
  },
  updateComponentTitle({ commit, dispatch }, { uuid, title }) {
    commit('setComponentTitle', {
      uuid,
      title,
    });
    dispatch('saveDisplayComponent', uuid);
  },
  async createDisplayComponent(
    { commit, dispatch },
    {
      componentName,
      parentUuid,
      title,
      params,
      state,
    },
  ) {
    const uuid = generateUuidv4();
    const newDisplayComponent = {
      componentUuid: uuid,
      componentName,
      parentUuid,
      params,
      title,
      state,
      rowDataTrigger: null,
    };

    commit('addDisplayComponent', newDisplayComponent);
    eventBus.$emit('new:display-component', { uuid });
    dispatch('saveDisplayComponent', uuid);

    return uuid;
  },
  async convertDisplayComponent(
    { commit, dispatch },
    {
      uuid, componentName, params, state,
    },
  ) {
    commit('setComponentName', { uuid, newComponentName: componentName });
    commit('setComponentState', { uuid, newState: state });
    commit('setComponentParams', { uuid, newParams: params });

    dispatch('saveDisplayComponent', uuid);
  },
  triggerComponentRowData({ commit, dispatch }, uuid) {
    commit('setComponentRowDataTrigger', { uuid, flag: Date.now() });
    dispatch('saveDisplayComponent', uuid);
  },
  async saveDisplayComponent({ state }, uuid) {
    const saveObject = { ...state.displayComponents[uuid] };
    delete saveObject.children;

    await componentStore.setItem(uuid, saveObject);
  },
  async removeDisplayComponentPersist(_, uuid) {
    await componentStore.removeItem(uuid);
    await deleteDataStore(uuid);
  },
  async loadAllDisplayComponents({ commit, dispatch }) {
    const displayComponents = [];
    await componentStore.iterate((value) => {
      displayComponents.push(value);
    });

    // Sort components in order of index so that the parent always precedes the children
    displayComponents.sort(
      (a, b) => a.displayComponentIndex - b.displayComponentIndex,
    );
    displayComponents.forEach((displayComponent) => {
      if (displayComponent.params.queryTemplate) {
        // Fix up the queryTemplate class
        // TODO: will need to support more classes in the future
        // eslint-disable-next-line no-param-reassign
        displayComponent.params.queryTemplate = new QueryTemplate(
          displayComponent.params.queryTemplate,
        );
      }
      commit('addDisplayComponent', displayComponent);

      // the displayComponentIndex likely needs updating
      dispatch('saveDisplayComponent', displayComponent.componentUuid);
    });
  },
  async exportDisplayComponents() {
    const displayComponents = [];
    await componentStore.iterate((value) => {
      displayComponents.push(value);
    });
    return displayComponents;
  },
  async importDisplayComponents(_, displayComponents) {
    await Promise.all(
      displayComponents.map(async (c) => {
        await componentStore.setItem(c.componentUuid, c);
      }),
    );
  },
};

const mutations = {
  setComponentTitle(state, { uuid, title }) {
    state.displayComponents[uuid].title = title;
  },
  setComponentRowDataTrigger(state, { uuid, flag }) {
    state.displayComponents[uuid].rowDataTrigger = flag;
  },
  setComponentState(state, { uuid, newState }) {
    state.displayComponents[uuid].state = newState;
  },
  setComponentParams(state, { uuid, newParams }) {
    state.displayComponents[uuid].params = newParams;
  },
  setComponentName(state, { uuid, newComponentName }) {
    state.displayComponents[uuid].componentName = newComponentName;
  },
  addDisplayComponent(state, displayComponent) {
    if (displayComponent.componentUuid in state.displayComponents) {
      return;
    }
    const newDisplayComponent = { ...displayComponent };
    state.displayComponents[displayComponent.componentUuid] = newDisplayComponent;
    newDisplayComponent.displayComponentIndex = state.displayComponentIndex;
    newDisplayComponent.children = [];

    state.displayComponentIndex += 1;

    if (newDisplayComponent.parentUuid === null) {
      state.rootDisplayComponents.push(newDisplayComponent);
    } else {
      const parentNode = state.displayComponents[newDisplayComponent.parentUuid];
      if (parentNode === undefined) {
        console.log('Parent node was not found.');
        state.rootDisplayComponents.push(newDisplayComponent);
      } else {
        parentNode.children.push(newDisplayComponent);
      }
    }
  },
  removeDisplayComponent(state, uuid) {
    const node = state.displayComponents[uuid];
    if (node === undefined) {
      console.log('Missing node?');
      return;
    }

    if (node.parentUuid === null) {
      state.rootDisplayComponents = state.rootDisplayComponents.filter(
        (e) => e.componentUuid !== uuid,
      );
    } else {
      const parentNode = state.displayComponents[node.parentUuid];
      if (parentNode !== undefined) {
        parentNode.children = parentNode.children.filter(
          (e) => e.componentUuid !== uuid,
        );
      }
    }

    delete state.displayComponents[uuid];
  },
};

const state = () => ({
  displayComponents: {},
  rootDisplayComponents: [],
  displayComponentIndex: 0, // Used to order components on save/load
});

export default {
  namespaced: true,
  state,
  getters,
  actions,
  mutations,
};
