<template>
  <v-menu
    v-model="showMenu"
    offset-y
    :close-on-content-click="false"
    disable-keys
  >
    <template #activator="{ on, attrs }">
      <v-btn
        :text="text"
        :small="small"
        :icon="icon"
        :tile="tile"
        v-bind="attrs"
        v-on="on"
      >
        <slot />
      </v-btn>
    </template>
    <v-list>
      <v-list-item link @click="onClickNewQuery">
        <v-list-item-title class="text-body-2">
          New query
        </v-list-item-title>
      </v-list-item>
      <v-divider />
      <v-list-item>
        <v-list-item-content>
          <v-text-field
            v-model="searchQuery"
            dense
            placeholder="Search queries"
            prepend-inner-icon="mdi-magnify"
          />
        </v-list-item-content>
      </v-list-item>
      <TemplateQuerySubMenu
        v-if="getViewMenuItems.length > 0"
        title="Views"
        :items="getViewMenuItems"
        :active="!!searchQuery"
        @close-menu="onCloseMenu"
      />
      <TemplateQuerySubMenu
        v-if="getQueryMenuItems.length > 0"
        title="Queries"
        :items="getQueryMenuItems"
        :active="!!searchQuery"
        @close-menu="onCloseMenu"
      />
    </v-list>
  </v-menu>
</template>
<script>
import { mapActions, mapGetters } from 'vuex';
import {
  createNewQueryComponent,
  createNewTemplateQueryComponent,
  templateQueriesAsObject,
} from '@/helpers/displayComponent';
import TemplateQuerySubMenu from '@/components/TemplateQuerySubMenu.vue';

export default {
  name: 'NewQueryButton',
  components: { TemplateQuerySubMenu },
  props: {
    text: Boolean,
    small: Boolean,
    icon: Boolean,
    tile: Boolean,
  },
  data: () => ({
    searchQuery: null,
    showMenu: false,
  }),
  computed: {
    ...mapGetters('queries', [
      'getViewTemplates',
      'getQueryTemplates',
      'getQueryOption',
    ]),
    getViewMenuItems() {
      const templates = this.getViewTemplates
        .filter((queryTemplate) => this.getQueryOption(queryTemplate.uuid).hide !== true);
      return this.filterItemsAsObject(templates);
    },
    getQueryMenuItems() {
      const templates = this.getQueryTemplates
        .filter((queryTemplate) => this.getQueryOption(queryTemplate.uuid).hide !== true);
      return this.filterItemsAsObject(templates);
    },
  },
  methods: {
    ...mapActions('displayComponent', [
      'updateComponentParams',
      'updateComponentTitle',
    ]),
    onCloseMenu() {
      this.showMenu = false;
    },
    filterItemsAsObject(items) {
      let itemResult = items;
      if (this.searchQuery) {
        const searchQuery = this.searchQuery.toLowerCase();
        itemResult = items.filter((item) => {
          const itemString = JSON.stringify([item.menu, item.summary]);
          return itemString.toLowerCase().includes(searchQuery);
        });
      }
      return templateQueriesAsObject(itemResult);
    },
    onClickNewQuery() {
      createNewQueryComponent('New query');
    },
    async onClickNewView(queryTemplate) {
      await createNewTemplateQueryComponent(
        queryTemplate.summary,
        queryTemplate,
        queryTemplate.getDefaultParams(),
        null,
        false,
        true,
      );
    },
  },
};
</script>

<style></style>
