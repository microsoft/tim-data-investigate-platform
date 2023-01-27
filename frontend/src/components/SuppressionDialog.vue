<template>
  <v-dialog
    v-model="dialog"
    max-width="1024px"
    persistent
    hide-overlay
    :retain-focus="false"
    no-click-animation
  >
    <v-card>
      <v-card-title class="popup-header">
        <span class="headline">Create Suppression</span>
      </v-card-title>
      <v-card-text>
        <v-container fluid>
          <v-form>
            <v-row dense>
              <v-col cols="8">
                <v-text-field
                  v-model="suppression.reason"
                  label="Justification"
                />
              </v-col>
              <v-col cols="4">
                <v-text-field
                  v-model="suppression.tags"
                  label="Tags"
                />
              </v-col>
            </v-row>
            <p class="text-subtitle-1">
              Select fields
            </p>
            <v-simple-table dense>
              <template #default>
                <thead>
                  <tr>
                    <th style="width: 300px;">
                      Column Name
                    </th>
                    <th style="width: 150px;">
                      Type
                    </th>
                    <th>Value</th>
                    <th style="width: 105px;">
                      Actions
                    </th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(condition, index) in suppression.conditions" :key="index">
                    <td>
                      <v-autocomplete
                        v-model="condition.column"
                        :items="getColumnNames"
                      />
                    </td>
                    <td>
                      <v-autocomplete
                        v-model="condition.type"
                        :items="['eq', 'ieq', 'contains', 'icontains',
                                 'startswith', 'istartswith', 'endswith', 'iendswith']"
                      />
                    </td>
                    <td>
                      <v-text-field
                        v-model="condition.match"
                      />
                    </td>
                    <td>
                      <v-btn
                        icon
                        color="pink"
                        @click="onDeleteCondition(index)"
                      >
                        <v-icon>mdi-delete</v-icon>
                      </v-btn>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <v-autocomplete
                        :key="suppressionPlaceholder"
                        placeholder="Select a column to match"
                        :items="getColumnNames"
                        @change="onChangePlaceHolder"
                      />
                    </td>
                  </tr>
                </tbody>
              </template>
            </v-simple-table>
          </v-form>
        </v-container>
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn
          color="blue darken-1"
          text
          @click="dialog = false"
        >
          Close
        </v-btn>
        <v-btn
          color="blue darken-1"
          text
          @click="onSaveSuppression()"
        >
          Save
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script>
import eventBus from '@/helpers/eventBus';
import { isEmptyValue } from '@/helpers/utils';
import { createSuppression } from '@/helpers/apiClient';

const EmptySuppression = () => ({
  conditions: [],
  tags: [],
  reason: '',
  id: null,
  type: null,
});

export default {
  name: 'SuppressionDialog',
  data: () => ({
    dialog: false,
    suppressionPlaceholder: 0,
    suppression: {
      data: {},
      onSuccess: null,
      ...EmptySuppression(),
    },
  }),
  computed: {
    getColumnNames() {
      return Object.keys(this.suppression.data)
        .filter((col) => !isEmptyValue(this.suppression.data[col]));
    },
  },
  mounted() {
    eventBus.$on('create:suppression-dialog', (event) => {
      this.dialog = true;

      this.suppression = { ...this.suppression, ...EmptySuppression() };

      this.suppression.data = event.data;
      this.suppression.id = event.id;
      this.suppression.type = event.type;
      this.suppression.onSuccess = event.onSuccess;
    });
  },
  methods: {
    onChangePlaceHolder(colName) {
      const value = this.suppression.data[colName];

      this.suppression.conditions.push({
        column: colName,
        type: 'eq',
        match: typeof value === 'object' ? JSON.stringify(value) : value,
      });
      this.suppressionPlaceholder += 1;
    },
    onDeleteCondition(index) {
      this.suppression.conditions.splice(index, 1);
    },
    async onSaveSuppression() {
      const conditions = this.suppression.conditions.map((condition) => ({
        conditionEntity: condition.column,
        conditionType: condition.type,
        conditionValue: condition.match,
      }));
      console.debug('Saving suppression...');

      eventBus.$emit('show:snackbar', {
        message: 'Creating and executing new suppression...',
      });

      try {
        await createSuppression(
          conditions,
          this.suppression.tags,
          this.suppression.reason,
          this.suppression.id,
        );
        eventBus.$emit('show:snackbar', {
          message: 'Suppression has successfully created and executed.',
          color: 'success',
          icon: 'mdi-check',
        });
        this.dialog = false;
      } catch (e) {
        eventBus.$emit('show:snackbar', {
          message: `Failed: ${e.toString()}`,
          color: 'error',
          icon: 'mdi-error',
        });
      }
    },
  },
};
</script>
