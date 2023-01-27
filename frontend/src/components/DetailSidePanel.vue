<template>
  <v-navigation-drawer
    :value="isSidePanelActive"
    absolute
    temporary
    right
    stateless
    :hide-overlay="true"
    width="900"
  >
    <v-container>
      <div class="text-right">
        <v-btn icon @click="$emit('close:side-panel')">
          <v-icon dark>
            mdi-close
          </v-icon>
        </v-btn>
      </div>
      <v-expansion-panels
        v-model="panel"
        accordion
        flat
        multiple
      >
        <v-expansion-panel>
          <v-expansion-panel-header>Result Details</v-expansion-panel-header>
          <v-divider class="mx-5 mb-3" />
          <v-expansion-panel-content>
            <dl>
              <template v-for="item in Object.keys(filteredData)">
                <dt :key="`${item}-dt`">
                  {{ item }}
                </dt>
                <dd :key="`${item}-dd`">
                  {{ yamlText(data[item]) }}
                </dd>
              </template>
            </dl>
          </v-expansion-panel-content>
        </v-expansion-panel>
      </v-expansion-panels>
    </v-container>
  </v-navigation-drawer>
</template>

<script>
import yaml from 'js-yaml';
import { isEmptyValue } from '@/helpers/utils';

export default {
  name: 'DetailSidePanel',
  props: {
    data: {
      type: Object,
      required: true,
    },
    isSidePanelActive: Boolean,
  },
  data: () => ({
    panel: [0, 1],
  }),
  computed: {
    filteredData() {
      if (!this.data) {
        return [];
      }

      return Object.keys(this.data).reduce((obj, v) => {
        if (!isEmptyValue(this.data[v])) {
          // eslint-disable-next-line no-param-reassign
          obj[v] = this.data[v];
        }
        return obj;
      }, {});
    },
  },
  mounted() {},
  methods: {
    yamlText: (data) => {
      if (!(typeof data === 'object')) {
        return data;
      }

      // eslint-disable-next-line no-control-regex
      // data = data.replace(/[\x00-\x08\x0b-\x1f\x7f-\x9f]/gu,'');
      return yaml.dump(data, {
        replacer: (key, value) => {
          if (typeof value === 'string') {
            // eslint-disable-next-line no-control-regex
            return value.replace(/[\x00-\x08\x0b-\x1f\x7f-\x9f]/gu, '');
          }
          return value;
        },
      });
    },
  },
};
</script>

<style scoped>
dt {
  margin-top: 5px;
  grid-column-start: 1;
  font-weight: 500;
  overflow-wrap: anywhere;
}

dd {
  margin-top: 5px;
  padding-left: 10px;
  grid-column-start: 2;
  overflow-wrap: anywhere;
  white-space: pre-wrap;
}

dl {
  grid-template-columns: 250px auto;
  display: grid;
  font-size: 0.9rem;
}

h3 {
  margin-top: 2rem;
  font-weight: 400;
}
.v-item--active .v-expansion-panel-content {
  max-height: calc(100vh - 200px);
  overflow-y: scroll;
}
</style>
