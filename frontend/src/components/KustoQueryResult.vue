<template>
  <div>
    <div class="px-3 d-flex">
      <NewQueryButton small text>
        <v-icon left small>
          mdi-plus
        </v-icon>
        New
      </NewQueryButton>
      <v-divider
        vertical
        style="height: 26px"
        class="mr-2"
      />
      <TimeSelection :time-range.sync="timeRange" />
      <v-divider
        vertical
        style="height: 26px"
        class="ml-2"
      />
      <v-btn
        text
        small
        :disabled="editQuery"
        @click="onClickRunQuery"
      >
        <v-icon left small>
          mdi-play
        </v-icon>
        Run Query
      </v-btn>
      <v-btn
        text
        small
        :disabled="editQuery"
        @click="onClickCloneQuery"
      >
        <v-icon left small>
          mdi-content-copy
        </v-icon>
        Clone
      </v-btn>
      <v-divider vertical style="height: 26px" />
      <v-btn
        v-if="!editQuery"
        text
        small
        @click="editQuery = true"
      >
        <v-icon left small>
          mdi-pencil
        </v-icon>
        Edit Query
      </v-btn>
      <v-btn
        v-if="editQuery"
        text
        small
        color="primary"
        @click="onClickSaveRunQuery"
      >
        <v-icon left small>
          mdi-play
        </v-icon>
        Save Changes & Run
      </v-btn>
      <v-btn
        v-if="editQuery"
        text
        small
        @click="onClickSaveQuery"
      >
        Save Changes
      </v-btn>
      <v-btn
        v-if="editQuery"
        text
        small
        @click="editQuery = false"
      >
        Cancel
      </v-btn>
    </div>
    <v-form
      v-if="editQuery && editParams"
      ref="form"
      class="mx-2"
    >
      <v-text-field v-model="editTitle" label="Summary" />
      <ClusterSelection
        :cluster.sync="editParams.cluster"
        :database.sync="editParams.database"
      />
      <p class="grey--text text--darken-1">
        Query
        <v-btn icon @click="helpDialog = true">
          <v-icon>mdi-help-circle-outline</v-icon>
        </v-btn>
      </p>
      <MonacoEditor
        v-model="editParams.query"
        style="
          width: 100%;
          height: 400px;
          border: 1px dotted darkgrey;
          margin-bottom: 20px;
          resize: vertical;
          overflow: hidden;
        "
        language="kusto"
        :options="options"
      />
    </v-form>
    <div>
      <v-alert
        v-if="error"
        type="error"
        text
        outlined
      >
        <pre class="code" style="white-space: pre-wrap">
          {{ error.toString() }}
        </pre>
      </v-alert>
      <KustoPivot
        v-if="rowDataTrigger"
        :uuid="uuid"
        @create:query-template="createQueryTemplate($event)"
      />
    </div>
    <QueryHelperDialog :dialog.sync="helpDialog" />
  </div>
</template>
<script>
import { mapActions, mapGetters } from 'vuex';
import {
  createNewQueryComponent,
  createNewTemplateQueryComponent,
  runNewQuery,
} from '@/helpers/displayComponent';
import KustoPivot from '@/components/grids/KustoPivot.vue';
import NewQueryButton from '@/components/NewQueryButton.vue';
import eventBus from '@/helpers/eventBus';
import MonacoEditor from '@/components/MonacoEditor.vue';
import TimeSelection from '@/components/TimeSelection.vue';
import QueryHelperDialog from '@/components/QueryHelperDialog.vue';
import ClusterSelection from '@/components/ClusterSelection.vue';

export default {
  name: 'KustoQueryResult',
  components: {
    ClusterSelection,
    QueryHelperDialog,
    TimeSelection,
    NewQueryButton,
    KustoPivot,
    MonacoEditor,
  },
  props: {
    uuid: {
      type: String,
      required: true,
    },
  },
  data: () => ({
    helpDialog: false,
    clusterSearchText: null,
    databaseSearchText: null,
    expandPanel: [0],
    editTitle: null,
    editParams: {
      query: '',
    },
    timeRange: null,
    editQuery: true,
    options: {
      tabSize: 2,
      minimap: {
        enabled: false,
      },
      lineNumbers: true,
      suggest: {
        enabled: false,
      },
      automaticLayout: true,
    },
  }),
  computed: {
    ...mapGetters('displayComponent', [
      'getComponentParams',
      'getComponentTitle',
      'getComponentState',
      'getComponentParentUuid',
      'getComponentRowDataTrigger',
    ]),
    title() {
      return this.getComponentTitle(this.uuid);
    },
    rowDataTrigger() {
      return this.getComponentRowDataTrigger(this.uuid);
    },
    error() {
      return this.getComponentState(this.uuid).error;
    },
    query() {
      return this.getComponentParams(this.uuid).query;
    },
    cluster() {
      return this.getComponentParams(this.uuid).cluster;
    },
    database() {
      return this.getComponentParams(this.uuid).database;
    },
  },
  watch: {
    editQuery(value) {
      if (value) {
        this.editParams = {
          cluster: this.cluster,
          database: this.database,
          query: this.query,
        };
        this.editTitle = this.title;
      } else {
        this.editParams = {};
        this.editTitle = null;
      }
    },
  },
  mounted() {
    if (this.editQuery) {
      this.editParams = {
        cluster: this.cluster,
        database: this.database,
        query: this.query,
      };
      this.editTitle = this.title;
    }
  },
  methods: {
    ...mapActions('displayComponent', [
      'updateComponentParams',
      'updateComponentTitle',
    ]),
    onClickSaveRunQuery() {
      if (!this.$refs.form.validate()) {
        return;
      }

      this.onClickSaveQuery();
      this.onClickRunQuery();
    },
    onClickSaveQuery() {
      if (!this.$refs.form.validate()) {
        return;
      }

      this.updateComponentTitle({
        uuid: this.uuid,
        title: this.editTitle,
      });
      this.updateComponentParams({
        uuid: this.uuid,
        cluster: this.editParams.cluster,
        database: this.editParams.database,
        query: this.editParams.query,
      });
      this.editQuery = false;
    },
    onClickNewQuery() {
      createNewQueryComponent('New query');
    },
    onClickNewView(queryTemplate) {
      createNewTemplateQueryComponent(
        queryTemplate.summary,
        queryTemplate,
        queryTemplate.getDefaultParams(),
        null,
        true,
      );
    },
    createQueryTemplate({ title, queryTemplate, params }) {
      createNewTemplateQueryComponent(
        title,
        queryTemplate,
        params,
        this.uuid,
        true,
      );
    },
    onClickRunQuery() {
      eventBus.$emit('show:snackbar', {
        message: 'Executing query...',
      });
      const parameters = {
        startTime: this.timeRange.getStartTime(),
        endTime: this.timeRange.getEndTime(),
      };
      runNewQuery(
        this.uuid,
        this.query,
        this.cluster,
        this.database,
        parameters,
      );
    },
    onClickCloneQuery() {
      createNewQueryComponent(
        `Copy of ${this.title}`,
        this.query,
        this.cluster,
        this.database,
      );
    },
  },
};
</script>

<style>
.code {
  font-family: Consolas, "Courier New", monospace;
  font-weight: normal;
  font-size: 0.75em;
  font-feature-settings: "liga" 0, "calt" 0;
  line-height: 19px;
  letter-spacing: 0;
}
</style>
