import Vue from 'vue';
import Vuex from 'vuex';
import columnViews from '@/store/modules/columnViews';
import displayComponent from '@/store/modules/displayComponent';
import queries from '@/store/modules/queries';

Vue.use(Vuex);

const debug = import.meta.env.NODE_ENV !== 'production';

export default new Vuex.Store({
  modules: {
    displayComponent,
    queries,
    columnViews,
  },
  strict: debug,
  //  plugins: debug ? [createLogger()] : []
});
