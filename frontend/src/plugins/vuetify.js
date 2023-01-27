import Vue from 'vue';
import Vuetify from 'vuetify/lib';
import Ripple from 'vuetify/lib/directives/ripple';

Vue.use(Vuetify, {
  directives: {
    Ripple,
  },
});

export default new Vuetify({});
