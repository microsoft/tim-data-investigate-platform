<template>
  <div>
    <div class="px-3 d-flex">
      <NewQueryButton small text>
        <v-icon left small>
          mdi-plus
        </v-icon>
        New
      </NewQueryButton>
      <v-divider vertical style="height: 26px" />
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
      <v-dialog v-model="convertWarning" width="500">
        <template #activator="{ on, attrs }">
          <v-btn
            text
            small
            :disabled="editQuery"
            v-bind="attrs"
            v-on="on"
          >
            Convert
          </v-btn>
        </template>
        <v-card>
          <v-card-title class="text-h5">
            Convert to custom query?
          </v-card-title>
          <v-card-text>
            This will convert the templated query into a custom query by making
            all parameters constant. This will allow you to modify the KQL
            directly. Note that this is permanent and cannot be
            undone.
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn
              color="darken-1"
              text
              @click="convertWarning = false"
            >
              Cancel
            </v-btn>
            <v-btn
              color="primary darken-1"
              text
              @click="onClickConvertQuery"
            >
              Convert
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
      <v-divider vertical style="height: 26px" />
      <v-btn
        v-if="!editQuery"
        text
        small
        @click="onClickEditQuery"
      >
        <v-icon left small>
          mdi-pencil
        </v-icon>
        Edit
      </v-btn>
      <v-btn
        v-if="!editQuery"
        text
        small
        @click="onClickShareQuery"
      >
        <v-icon left small>
          mdi-share
        </v-icon>
        Share Link
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
        Save & Run
      </v-btn>
      <v-btn
        v-if="editQuery"
        text
        small
        @click="onClickSaveQuery"
      >
        <v-icon left small>
          mdi-content-save
        </v-icon>
        Save
      </v-btn>
      <v-btn
        v-if="editQuery"
        text
        small
        @click="onClickCancelEdit"
      >
        Cancel
      </v-btn>
    </div>
    <v-form
      v-if="editQuery && editParams"
      ref="form"
      class="mx-2"
    >
      <v-text-field
        v-model="editTitle"
        label="Summary"
        prepend-icon="mdi-refresh"
        @click:prepend="onClickRefreshTitle"
      />
      <v-row dense>
        <v-col
          v-for="(field, fieldKey) in {
            ...queryTemplate.params,
            ...queryTemplate.fields,
          }"
          :key="fieldKey"
          cols="4"
        >
          <v-select
            v-if="field.type === 'array'"
            v-model="editParams[fieldKey]"
            :items="field.values"
            :hint="field.hint"
            persistent-hint
            :multiple="field.multiple === true"
            :rules="[...(field.optional !== true ? [rules.required] : [])]"
          >
            <template #label>
              {{ fieldKey }}
              <span v-if="field.optional !== true" class="red--text"> *</span>
            </template>
            <template
              v-if="field.multiple === true"
              #selection="{ item, index }"
            >
              <span
                v-if="index === 0 || editParams[fieldKey].length <= 5"
                class="mr-1"
              >
                {{ item }},
              </span>
              <span
                v-if="index === 1 && editParams[fieldKey].length > 5"
                class="grey--text caption mx-1"
              >
                (+{{ editParams[fieldKey].length - 1 }} others)
              </span>
            </template>
            <template v-if="field.multiple === true" #prepend-item>
              <v-list-item ripple @click="clearFieldSelection(fieldKey, field)">
                <v-list-item-action>
                  <v-icon
                    :color="editParams[fieldKey].length > 0 ? 'indigo darken-4' : ''"
                  >
                    <template v-if="editParams[fieldKey].length > 0">
                      <template v-if="editParams[fieldKey].length === field.values.length">
                        mdi-close-box
                      </template>
                      <template v-else>
                        mdi-minus-box
                      </template>
                    </template>
                    <template v-else>
                      mdi-checkbox-blank-outline
                    </template>
                  </v-icon>
                </v-list-item-action>
                <v-list-item-content>
                  <v-list-item-title>Select All</v-list-item-title>
                </v-list-item-content>
              </v-list-item>
              <v-divider class="mt-2" />
            </template>
          </v-select>
          <v-select
            v-else-if="field.type === 'match' && typeof getFieldWithCache(fieldKey) === 'object'"
            v-model="editParams[fieldKey]"
            :items="getFieldWithCache(fieldKey)"
            item-value="value"
            :item-text="(v) => `${v.column}: ${v.value}`"
            :hint="field.hint"
            :rules="[...(field.optional !== true ? [rules.required] : [])]"
            persistent-hint
          >
            <template #label>
              {{ fieldKey }}
              <span v-if="field.optional !== true" class="red--text"> *</span>
            </template>
          </v-select>
          <v-combobox
            v-else-if="field.type === 'multiple'"
            v-model="editParams[fieldKey]"
            :items="getFieldWithCache(fieldKey)"
            :hint="field.hint"
            :rules="[...(field.optional !== true ? [rules.required] : [])]"
            :delimiters="[',', ';']"
            persistent-hint
            multiple
            clearable
            small-chips
            append-icon=""
          >
            <template #label>
              {{ fieldKey }}
              <span v-if="field.optional !== true" class="red--text"> *</span>
            </template>
          </v-combobox>
          <v-switch
            v-else-if="field.type === 'boolean'"
            v-model="editParams[fieldKey]"
            :label="fieldKey"
            :hint="field.hint"
            persistent-hint
          />
          <v-text-field
            v-else
            v-model.trim="editParams[fieldKey]"
            :hint="field.hint"
            persistent-hint
            :rules="[...(field.optional !== true ? [rules.required] : [])]"
          >
            <template #label>
              {{ fieldKey }}
              <span v-if="field.optional !== true" class="red--text"> *</span>
            </template>
          </v-text-field>
        </v-col>
      </v-row>
      <div class="d-flex">
        <div class="text-subtitle-1">
          Preview Query
        </div>
        <a class="text-body-2 ml-auto" @click="viewQuery = !viewQuery">
          {{ viewQuery ? "hide" : "show" }}
        </a>
      </div>
      <v-divider class="mb-3" />
      <v-textarea
        v-show="viewQuery"
        rows="20"
        :value="getQuery"
        readonly
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
  </div>
</template>

<script>
import KustoPivot from '@/components/grids/KustoPivot.vue';
import { mapActions, mapGetters } from 'vuex';
import {
  createNewQueryComponent,
  createNewTemplateQueryComponent,
  convertToCustomQuery,
  runTemplateQuery,
} from '@/helpers/displayComponent';
import NewQueryButton from '@/components/NewQueryButton.vue';
import eventBus from '@/helpers/eventBus';
import { generateURL } from '@/helpers/utils';

export default {
  name: 'TemplateQueryResult',
  components: {
    NewQueryButton,
    KustoPivot,
  },
  props: {
    uuid: {
      type: String,
      required: true,
    },
  },
  data: () => ({
    viewQuery: false,
    rules: {
      required: (value) => !!value || 'Required.',
    },
    querySnackbar: {
      enabled: false,
      text: null,
      color: null,
    },
    editParams: null,
    editTitle: null,
    convertWarning: false,
    cacheVariable: {},
    autoExecute: true,
  }),
  computed: {
    ...mapGetters('displayComponent', [
      'getComponentParams',
      'getComponentTitle',
      'getComponentState',
      'getComponentParentUuid',
      'getComponentRowDataTrigger',
    ]),
    params() {
      return this.getComponentParams(this.uuid).inParams;
    },
    title() {
      return this.getComponentTitle(this.uuid);
    },
    editQuery() {
      return this.getComponentState(this.uuid).editQuery;
    },
    queryTemplate() {
      return this.getComponentParams(this.uuid).queryTemplate;
    },
    rowDataTrigger() {
      return this.getComponentRowDataTrigger(this.uuid);
    },
    parentUuid() {
      return this.getComponentParentUuid(this.uuid);
    },
    getQuery() {
      return this.queryTemplate.buildQuery(this.editParams);
    },
    error() {
      return this.getComponentState(this.uuid).error;
    },
  },
  watch: {
    editQuery(value) {
      if (value) {
        this.editParams = { ...this.params };
        this.editTitle = this.title;
      } else {
        this.editParams = null;
        this.editTitle = null;
      }
      this.cacheVariable = {};
    },
  },
  mounted() {
    if (this.editQuery) {
      this.editParams = { ...this.params };
      this.editTitle = this.title;
    }
    window.addEventListener('keydown', this.onKeypressListener);
    window.addEventListener('keyup', this.onKeypressListener);
  },
  destroyed() {
    window.removeEventListener('keydown', this.onKeypressListener);
    window.removeEventListener('keyup', this.onKeypressListener);
  },
  methods: {
    ...mapActions('displayComponent', [
      'updateComponentState',
      'updateComponentTitle',
      'updateComponentParams',
    ]),
    onClickRefreshTitle() {
      this.editTitle = this.queryTemplate.buildSummary(this.editParams);
    },
    onKeypressListener(e) {
      if (e.key === 'Shift') {
        if (e.type === 'keydown') {
          this.autoExecute = false;
        } else if (e.type === 'keyup') {
          this.autoExecute = true;
        }
      }
    },
    getFieldWithCache(fieldKey) {
      if (!(fieldKey in this.cacheVariable)) {
        this.cacheVariable[fieldKey] = this.editParams[fieldKey];
      }
      return this.cacheVariable[fieldKey];
    },
    onClickEditQuery() {
      this.updateComponentState({
        uuid: this.uuid,
        editQuery: true,
      });
    },
    onClickCancelEdit() {
      this.updateComponentState({
        uuid: this.uuid,
        editQuery: false,
      });
    },
    onClickConvertQuery() {
      convertToCustomQuery(this.uuid, this.queryTemplate, this.params);
      this.convertWarning = false;
    },
    onClickSaveQuery() {
      if (this.$refs.form.validate()) {
        this.updateComponentTitle({
          uuid: this.uuid,
          title: this.editTitle,
        });
        this.updateComponentParams({
          uuid: this.uuid,
          inParams: { ...this.editParams },
        });
        this.updateComponentState({
          uuid: this.uuid,
          editQuery: false,
        });
        return true;
      }
      eventBus.$emit('show:snackbar', {
        message: 'Unable to save query due to validation errors.',
        color: 'error',
        icon: 'mdi-alert',
      });
      return false;
    },
    onClickSaveRunQuery() {
      if (this.onClickSaveQuery()) {
        this.onClickRunQuery();
      }
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
        false,
      );
    },
    async onClickShareQuery() {
      const buf = btoa(JSON.stringify(this.params));
      const route = this.$router.resolve({
        name: 'ShareQuery',
        params: { uuid: this.queryTemplate.uuid },
        query: { p: buf, execute: 0 },
      });

      await navigator.clipboard.writeText(generateURL(route.href));

      eventBus.$emit('show:snackbar', {
        message: 'Shared link has been saved to the clipboard.',
      });
    },
    clearFieldSelection(fieldKey, field) {
      this.editParams[fieldKey] = this.editParams[fieldKey].length === field.values.length
        ? []
        : field.values.slice();
    },
    createQueryTemplate({ title, queryTemplate, params }) {
      createNewTemplateQueryComponent(
        title,
        queryTemplate,
        params,
        this.uuid,
        this.autoExecute,
      );
    },
    onClickRunQuery() {
      runTemplateQuery(this.uuid);
      eventBus.$emit('show:snackbar', {
        message: 'Executing query...',
      });
    },
    onClickCloneQuery() {
      createNewTemplateQueryComponent(
        `Copy of ${this.title}`,
        this.queryTemplate,
        this.params,
        this.parentUuid,
        false,
        true,
      );
    },
  },
};
</script>

<style></style>
