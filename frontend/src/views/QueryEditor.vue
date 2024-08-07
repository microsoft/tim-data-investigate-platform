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
        class="ml-1"
        depressed
        small
        :disabled="selected.length === 0"
        @click="onClickExport"
      >
        <v-icon small dark left>mdi-export</v-icon>
        Export ({{ selected.length }})
      </v-btn>
      <v-btn
        class="ml-1"
        depressed
        small
        @click="triggerFileInput"
      >
        <v-icon small dark left>mdi-import</v-icon>
        Import
      </v-btn>
      <input type="file" ref="fileInput" @change="onClickImport" accept=".yaml,.yml,.zip" style="display: none" multiple>
      <v-btn
        v-if="showDeleted"
        class="ml-1"
        depressed
        small
        :disabled="selectedRestore.length === 0 || selectedHasManaged"
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
            v-model="showDeleted"
            label="Show deleted"
            class="d-md-inline-block"
          >
          </v-switch>
          <v-text-field
            v-model="search"
            prepend-inner-icon="mdi-magnify"
            label="Filter"
            class="mx-4"
          ></v-text-field>
        </template>
        <template v-slot:item.data-table-select="{ item, isSelected, select }">
          <v-simple-checkbox
            :value="isSelected"
            @input="select($event)"
          ></v-simple-checkbox>
        </template>
        <template v-slot:item.isHidden="{ item }">
          <v-switch
            :input-value="isQueryHidden(item.uuid)"
            @change="onHideQuery(item.uuid, $event)"
          ></v-switch>
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
import { queryDelete, queryRestore, queryCreate } from '@/helpers/apiClient';
import eventBus from '@/helpers/eventBus';
import { mapActions, mapGetters } from 'vuex';
import yaml from 'js-yaml';
import { zip, unzip } from 'fflate';
import { generateUuidv4 } from '@/helpers/utils';

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
      { text: 'Path', value: 'path' },
      // { text: 'Cluster', value: 'cluster' },
      { text: 'Created By', value: 'createdBy' },
      { text: 'Updated By', value: 'updatedBy' },
      { text: 'Last Updated', value: 'updated' },
      { text: 'Managed', value: 'isManaged' },
      { text: 'Hidden', value: 'isHidden' },
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
    triggerFileInput() {
      this.$refs.fileInput.click();
      this.$refs.fileInput.value = "";
    },
    async onClickExport() {
      const files = {};
      this.selected.forEach(query => {
        const yamlContent = yaml.dump(query);
        const fileName = `${query.uuid}.yaml`;
        // Convert string content to Uint8Array
        files[fileName] = new TextEncoder().encode(yamlContent);
      });

      zip(files, (err, zippedData) => {
        if (err) {
          console.error('Error zipping files:', err);
          return;
        }
        const blob = new Blob([zippedData], { type: 'application/zip' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = 'queryexport.zip';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
      });

      await this.onReloadQueries();
      await this.reloadQueries();
      this.selected = [];
    },
    onClickImport(event) {
      const files = event.target.files;
      if (!files.length) return;

      Array.from(files).forEach(file => {
        if (file.name.endsWith('.zip')) {
          // Handle zip files
          const reader = new FileReader();
          reader.onload = (e) => {
            const zipData = new Uint8Array(e.target.result);
            unzip(zipData, (err, unzipped) => {
              if (err) {
                console.error('Error unzipping file:', err);
                return;
              }
              Object.keys(unzipped).forEach(filename => {
                if (filename.endsWith('.yaml') || filename.endsWith('.yml')) {
                  const yamlContent = new TextDecoder().decode(unzipped[filename]);
                  this.processYamlContent(yamlContent, filename);
                }
              });
            });
          };
          reader.readAsArrayBuffer(file);
        } else {
          // Handle YAML files directly
          const reader = new FileReader();
          reader.onload = (e) => {
            const yamlContent = e.target.result;
            this.processYamlContent(yamlContent, file.name);
          };
          reader.readAsText(file);
        }
      });
    },
    // TODO: standardize YAML validation that is occuring both here and in the CreateQueryDialog.vue; move to backend?
    async processYamlContent(yamlContent, filename) {
      let parsedYaml;
      try {
        parsedYaml = yaml.load(yamlContent);
        if (typeof parsedYaml !== 'object' || parsedYaml === null) {
          throw new Error(`Error parsing YAML from "${filename}" - resulted in non-object or null value`);
        }

        const requiredFields = ['name', 'queryType', 'menu', 'query', 'summary', 'cluster', 'database'];
        const missingFields = requiredFields.filter(field => !parsedYaml[field]);

        if (parsedYaml.queryType === 'query' && !parsedYaml.fields) {
          missingFields.push('fields');
        }

        if (missingFields.length > 0) {
          throw new Error(`Missing required field(s): "${missingFields.join(', ')}" for imported query "${filename}"`);
        }

        if (parsedYaml.fields != null && (typeof parsedYaml.fields !== 'object' || !Object.keys(parsedYaml.fields).every(key => parsedYaml.fields[key]?.hasOwnProperty('type')))) {
          console.log(parsedYaml);
          console.log(parsedYaml.fields);
          throw new Error('Check Fields YAML; Each field key must at least have type defined');
        }

        if (parsedYaml.params != null && (typeof parsedYaml.params !== 'object' || !Object.keys(parsedYaml.params).every(key => parsedYaml.params[key]?.hasOwnProperty('type')))) {
          throw new Error('Check Params YAML; Each parameter key must at least have type defined');
        }

        if (parsedYaml.columns != null && (typeof parsedYaml.columns !== 'object' || !Object.keys(parsedYaml.columns).every(key => parsedYaml.columns[key] !== null))) {
          throw new Error('Check Columns YAML; one or more YAML keys not converted to non-null object');
        }

        parsedYaml.uuid = generateUuidv4();
        parsedYaml.isManaged = "false";

        eventBus.$emit('show:snackbar', {
          message: `Importing query "${parsedYaml.name}" from ${filename} ...`,
        });

        await queryCreate(parsedYaml);
        await this.onReloadQueries();
        await this.reloadQueries();
        this.selected = [];

        const msg = `Successfully imported query "${parsedYaml.name}" from "${filename}"`
        eventBus.$emit('show:snackbar', {
          message: msg,
          color: 'success',
          icon: 'mdi-check',
        });
        console.log(msg);
      } catch (e) {
        console.error(e, e.stack);
        eventBus.$emit('show:snackbar', {
          message: e.message,
          color: 'error',
          icon: 'mdi-alert',
        });
        return;
      }
    },
  },
};
</script>

<style></style>
