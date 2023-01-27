<template>
  <v-container>
    <v-textarea
      v-model="jsonBlob"
      :rows="10"
      label="Settings (JSON)"
      outlined
    />
    <v-btn @click="onClickExport">
      <v-icon left>
        mdi-export
      </v-icon>
      Export
    </v-btn>
    <v-btn
      class="ml-3"
      :disabled="jsonBlob === null || jsonBlob.length === 0 || getJsonBlobAsObject === null"
      @click="onClickImport"
    >
      <v-icon left>
        mdi-import
      </v-icon>
      Import
    </v-btn>
  </v-container>
</template>

<script>
import { mapActions } from 'vuex';
import eventBus from '@/helpers/eventBus';

export default {
  name: 'ExportImport',
  data: () => ({
    jsonBlob: null,
  }),
  computed: {
    getJsonBlobAsObject() {
      try {
        return JSON.parse(this.jsonBlob);
      } catch (e) {
        return null;
      }
    },
  },
  methods: {
    ...mapActions('displayComponent', [
      'importDisplayComponents',
      'exportDisplayComponents',
    ]),
    async onClickImport() {
      await this.importDisplayComponents(this.getJsonBlobAsObject);

      eventBus.$emit('show:snackbar', {
        message:
          'All settings have been imported. Refresh the browser for changes.',
      });
    },
    async onClickExport() {
      this.jsonBlob = JSON.stringify(await this.exportDisplayComponents());

      await navigator.clipboard.writeText(this.jsonBlob);

      eventBus.$emit('show:snackbar', {
        message: 'All settings have been exported and saved to your clipboard.',
      });
    },
  },
};
</script>

<style></style>
