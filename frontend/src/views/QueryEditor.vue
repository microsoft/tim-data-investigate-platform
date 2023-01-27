<template>
  <v-container class="fill-height" fluid>
    <div
      style="
        width: 100%;
        height: 100%;
        margin-left: 56px;
        padding-left: 2em;
        padding-right: 12em;
      "
    >
      <v-btn
        color="primary"
        class="ml-1"
        depressed
        small
        @click="onClickCreateQuery"
      >
        <v-icon
          small
          dark
          left
        >
          mdi-plus-circle
        </v-icon>
        Create
      </v-btn>
      <v-btn
        class="ml-1"
        depressed
        small
        :disabled="selectedDelete.length === 0 || selectedHasManaged"
        @click="onClickDelete"
      >
        <v-icon
          small
          dark
          left
        >
          mdi-delete
        </v-icon>
        Delete ({{ selectedDelete.length }})
      </v-btn>
      <v-btn
        v-if="showDeleted"
        class="ml-1"
        depressed
        small
        :disabled="selectedDelete.length === 0 || selectedHasManaged"
        @click="onClickRestore"
      >
        <v-icon
          small
          dark
          left
        >
          mdi-auto-fix
        </v-icon>
        Restore ({{ selectedRestore.length }})
      </v-btn>
      <v-data-table
        v-model="selected"
        :headers="headers"
        :items="filteredQueries"
        :search="search"
        sort-by="name"
        show-select
        item-key="uuid"
      >
        <template #item.name="{ item }">
          <a
            v-if="item.isDeleted !== true"
            href="javascript:void(0)"
            @click="onClickEditQuery(item)"
          >{{ item.name }}</a>
          <span v-else class="grey--text lighten-1">{{ item.name }}</span>
        </template>
        <template v-slot:top>
          <v-switch
            label="Show deleted"
            v-model="showDeleted"
          >
          </v-switch>
          <v-text-field
            prepend-inner-icon="mdi-magnify"
            v-model="search"
            label="Filter"
            class="mx-4"
          ></v-text-field>
        </template>
      </v-data-table>
    </div>
    <CreateQueryDialog
      v-if="createQueryDialog"
      :dialog.sync="createQueryDialog"
      :edit-query="editQuery"
      @reload:queries="onReloadQueries"
    />
  </v-container>
</template>

<script>
import CreateQueryDialog from '@/components/CreateQueryDialog.vue';
import { queryDelete, queryRestore } from '@/helpers/apiClient';
import { mapActions, mapGetters } from 'vuex';

export default {
  name: 'QueryEditor',
  components: {
    CreateQueryDialog,
  },
  data: () => ({
    queries: [],
    createQueryDialog: false,
    editQuery: null,
    showDeleted: false,
    search: null,
    headers: [
      { text: 'Name', value: 'name' },
      { text: 'Type', value: 'queryType' },
      { text: 'Menu text', value: 'menu' },
      { text: 'Last Updated', value: 'updated' },
      { text: 'Path', value: 'path' },
      { text: 'Cluster', value: 'cluster' },
    ],
    selected: [],
  }),
  computed: {
    ...mapGetters('queries', [
      'getQueryOption',
    ]),
    filteredQueries() {
      if (this.showDeleted) {
        return this.queries;
      }
      return this.queries.filter((query) => query.isDeleted !== true);
    },
    selectedRestore() {
      return this.selected.filter((query) => query.isDeleted === true);
    },
    selectedDelete() {
      return this.selected.filter((query) => query.isDeleted !== true);
    },
    selectedHasManaged() {
      return this.selected.some((query) => query.isManaged === true);
    },
  },
  async mounted() {
    await this.onReloadQueries();
  },
  methods: {
    ...mapActions('queries', [
      'getQueries',
      'reloadQueries',
      'updateQueryOption',
    ]),
    async onReloadQueries() {
      this.queries = await this.getQueries(true);
    },
    onClickEditQuery(query) {
      this.editQuery = { ...query };
      this.createQueryDialog = true;
    },
    onClickCreateQuery() {
      this.editQuery = null;
      this.createQueryDialog = true;
    },
    async onHideQuery(uuid, value) {
      await this.updateQueryOption({ uuid, queryOption: { hide: value } });
    },
    async onClickRestore() {
      await Promise.all(
        this.selectedRestore.map((query) => queryRestore(query.uuid)),
      );
      await this.onReloadQueries();
      await this.reloadQueries();
      this.selected = [];
    },
    async onClickDelete() {
      await Promise.all(
        this.selectedDelete.map((query) => queryDelete(query.uuid)),
      );
      await this.onReloadQueries();
      await this.reloadQueries();
      this.selected = [];
    },
    isQueryHidden(uuid) {
      return this.getQueryOption(uuid).hide === true;
    },
  },
};
</script>

<style></style>
