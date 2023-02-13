import Vue from 'vue';
import Vuex from 'vuex';
import columnViews from '@/store/modules/columnViews';
import displayComponent from '@/store/modules/displayComponent';
import queries from '@/store/modules/queries';
import runtimeConfig from '@/helpers/runtimeConfig';

Vue.use(Vuex);

const debug = runtimeConfig.nodeEnv;

export default new Vuex.Store({
  modules: {
    displayComponent,
    queries,
    columnViews,
  },
  strict: debug,
  //  plugins: debug ? [createLogger()] : []
});
