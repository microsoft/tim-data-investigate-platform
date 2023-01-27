import '@mdi/font/css/materialdesignicons.css';
import Vue from 'vue';
import App from '@/App.vue';
import vuetify from '@/plugins/vuetify';
import router from '@/router/index';
import store from '@/store/index';

Vue.config.productionTip = false;
Vue.config.performance = true;

new Vue({
  vuetify,
  router,
  store,
  render: (h) => h(App),
  icons: {
    iconfont: 'mdi',
  },
}).$mount('#app');
