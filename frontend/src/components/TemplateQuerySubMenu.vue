<template>
  <v-list class="pl-4">
    <v-list-group no-action :value="active">
      <template #activator>
        <v-list-item-content>
          <v-list-item-title>{{ title }}</v-list-item-title>
        </v-list-item-content>
      </template>

      <TemplateQuerySubMenu
        v-for="item in getSubMenuItems"
        :key="`submenu-${item.title}`"
        :items="item.items"
        :title="item.title"
        :active="active"
        v-on="$listeners"
      />
      <v-list-item
        v-for="item in getMenuItems"
        :key="`${item.uuid}`"
        link
        class="pl-8"
        @click="onClickNewView(item)"
        v-on="$listeners"
      >
        <v-list-item-title class="text-body-2">
          {{ item.menu }}
        </v-list-item-title>
      </v-list-item>
    </v-list-group>
  </v-list>
</template>
<script>
import { mapActions } from 'vuex';
import { createNewTemplateQueryComponent } from '@/helpers/displayComponent';

export default {
  name: 'TemplateQuerySubMenu',
  props: {
    title: {
      type: String,
      required: true,
    },
    items: {
      type: Array,
      required: true,
    },
    active: Boolean,
  },
  computed: {
    getMenuItems() {
      return this.items.filter((v) => v.items === undefined);
    },
    getSubMenuItems() {
      return this.items.filter(
        (v) => Array.isArray(v.items) && v.items.length > 0,
      );
    },
  },
  methods: {
    ...mapActions('displayComponent', [
      'updateComponentParams',
      'updateComponentTitle',
    ]),
    async onClickNewView(queryTemplate) {
      await createNewTemplateQueryComponent(
        queryTemplate.summary,
        queryTemplate,
        queryTemplate.getDefaultParams(),
        null,
        false,
        true,
      );
      this.$emit('close-menu');
    },
  },
};
</script>

<style></style>
