<template>
  <v-dialog
    :value="dialog"
    persistent
    max-width="800px"
  >
    <v-card>
      <v-card-title>
        <span class="headline">{{ editQuery ? "Edit" : "Create" }} Query</span>
      </v-card-title>
      <v-card-text>
        <v-container fluid>
          <v-form ref="form" :disabled="query.isManaged === true">
            <v-row v-if="query.isManaged === true">
              <p class="red--text text--lighten-3">
                This query is being managed by source control.
              </p>
            </v-row>
            <v-row>
              <v-text-field
                v-model="query.name"
                label="Name"
                filled
                required
                :rules="[(v) => !!v || 'Name is required']"
              />
            </v-row>
            <v-row>
              <v-radio-group v-model="query.queryType" mandatory>
                <template #label>
                  Select type of query
                </template>
                <v-radio label="View" value="view" />
                <v-radio label="Query" value="query" />
              </v-radio-group>
            </v-row>
            <v-row>
              <v-text-field
                v-model="query.menu"
                label="Menu text"
                :rules="[(v) => !!v || 'Menu text is required']"
                filled
                hint="e.g. Show all children processes"
                required
              />
            </v-row>
            <v-row>
              <v-text-field
                v-model="query.summary"
                label="Summary text"
                :rules="[(v) => !!v || 'Summary text is required']"
                filled
                :hint="'Supports {{variable}} e.g. Timeline for {{MachineId}}'"
                required
              />
            </v-row>
            <v-row>
              <v-combobox
                v-model="query.path"
                label="Path"
                hint="The submenu path for this menu item e.g. Machine, Windows"
                chips
                filled
                clearable
                multiple
              />
            </v-row>
            <v-row>
              <v-text-field
                v-model="query.cluster"
                label="Cluster"
                :rules="[(v) => !!v || 'Cluster is required']"
                filled
                required
                :hint="'Domain name for cluster. Supports {{variable}} e.g. {{Cluster}}'"
              />
            </v-row>
            <v-row>
              <v-text-field
                v-model="query.database"
                label="Database"
                :rules="[(v) => !!v || 'Database is required']"
                filled
                required
              />
            </v-row>
            <v-row>
              <v-text-field
                v-model="query.columnId"
                label="Column Id"
                filled
                hint="Used as a unique identifier for each row e.g. EventId"
              />
            </v-row>
          </v-form>
          <v-row>
            <p>Params</p>
            <MonacoEditor
              v-model="editorParams"
              style="
                width: 100%;
                height: 200px;
                border: 1px dotted darkgrey;
                margin-bottom: 20px;
              "
              language="yaml"
              :options="{ ...options, readOnly: query.isManaged === true }"
            />
            <v-alert
              v-if="errorEditorParams"
              text
              type="error"
              style="width: 100%"
            >
              {{ errorEditorParams }}
            </v-alert>
          </v-row>
          <v-row v-if="query.queryType === 'query'">
            <p>Fields</p>
            <MonacoEditor
              v-model="editorFields"
              style="
                width: 100%;
                height: 200px;
                border: 1px dotted darkgrey;
                margin-bottom: 20px;
              "
              language="yaml"
              :options="options"
            />
            <v-alert
              v-if="errorEditorFields"
              text
              type="error"
              style="width: 100%"
            >
              {{ errorEditorFields }}
            </v-alert>
          </v-row>
          <v-row>
            <p>Column customisation</p>
            <MonacoEditor
              v-model="editorColumns"
              style="
                width: 100%;
                height: 200px;
                border: 1px dotted darkgrey;
                margin-bottom: 20px;
              "
              language="yaml"
              :options="{ ...options, readOnly: query.isManaged === true }"
            />
            <v-alert
              v-if="errorEditorColumns"
              text
              type="error"
              style="width: 100%"
            >
              {{ errorEditorColumns }}
            </v-alert>
          </v-row>
          <v-row>
            <p>Query</p>
            <MonacoEditor
              v-model="query.query"
              style="width: 100%; height: 200px; border: 1px dotted darkgrey"
              language="plaintext"
              :options="{ ...options, readOnly: query.isManaged === true }"
            />
          </v-row>
          <v-row>
            <v-alert
              v-if="creationError"
              text
              type="error"
              style="width: 100%; margin-top: 20px"
            >
              {{ creationError }}
            </v-alert>
          </v-row>
        </v-container>
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn
          color="blue darken-1"
          text
          @click="closeDialog()"
        >
          Close
        </v-btn>
        <v-btn
          color="blue darken-1"
          text
          :disabled="query.isManaged === true"
          @click="onCreateQuery()"
        >
          {{ editQuery ? "Save" : "Create" }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script>
import MonacoEditor from 'vue-monaco';
import yaml from 'js-yaml';
import { queryCreate, queryUpdate } from '@/helpers/apiClient';
import { mapActions } from 'vuex';
import eventBus from '@/helpers/eventBus';
import { generateUuidv4 } from '@/helpers/utils';

export default {
  name: 'CreateQueryDialog',
  components: {
    MonacoEditor,
  },
  props: {
    dialog: Boolean,
    editQuery: {
      type: Object,
      required: true,
    },
  },
  data: () => ({
    query: {
      query: '',
    },
    editorParams: '',
    errorEditorParams: null,
    editorFields: '',
    errorEditorFields: null,
    editorColumns: '',
    errorEditorColumns: null,
    creationError: null,
    options: {
      tabSize: 2,
      minimap: {
        enabled: false,
      },
      lineNumbers: false,
      suggest: {
        enabled: false,
      },
    },
  }),
  watch: {
    editorParams() {
      this.errorEditorParams = null;
    },
    editorFields() {
      this.errorEditorFields = null;
    },
    editorColumns() {
      this.errorEditorColumns = null;
    },
  },
  mounted() {
    if (this.editQuery !== null) {
      this.query = { ...this.editQuery };
      this.editorParams = yaml.dump(this.query.params);
      this.editorFields = yaml.dump(this.query.fields);
      this.editorColumns = yaml.dump(this.query.columns);
    }
  },
  methods: {
    ...mapActions('queries', ['reloadQueries']),
    async onCreateQuery() {
      let hasError = false;

      if (!this.query.query) {
        hasError = true;
        this.creationError = 'Missing query';
      }

      try {
        this.query.fields = yaml.load(this.editorFields);
      } catch (err) {
        hasError = true;
        this.errorEditorFields = err.message;
      }

      try {
        this.query.params = yaml.load(this.editorParams);
      } catch (err) {
        hasError = true;
        this.errorEditorParams = err.message;
      }

      try {
        this.query.columns = yaml.load(this.editorColumns);
      } catch (err) {
        hasError = true;
        this.errorEditorColumns = err.message;
      }

      if (!hasError && this.$refs.form.validate()) {
        try {
          eventBus.$emit('show:snackbar', {
            message: 'Saving query...',
          });

          if (this.editQuery !== null) {
            await queryUpdate(this.query.uuid, this.query);
          } else {
            this.query.uuid = generateUuidv4();
            await queryCreate(this.query);
          }
          await this.reloadQueries();
          this.$emit('reload:queries');
          this.closeDialog();

          eventBus.$emit('show:snackbar', {
            message: 'Successfully saved query.',
            color: 'success',
            icon: 'mdi-check',
          });
        } catch (err) {
          this.creationError = err.response.data.error;

          eventBus.$emit('show:snackbar', {
            message: err.response.data.error,
            color: 'error',
            icon: 'mdi-alert',
          });
        }
      }
    },
    closeDialog() {
      this.$emit('update:dialog', false);
    },
  },
};
</script>
<style>
.editor {
  width: 600px;
  height: 800px;
}
</style>
