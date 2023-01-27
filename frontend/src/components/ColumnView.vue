<template>
  <v-row align="baseline">
    <v-col class="py-1">
      <v-autocomplete
        :items="getAllColumnViews"
        dense
        hide-details
        label="Column view"
        :value="selectedUuid"
        :search-input.sync="search"
        item-value="uuid"
        item-text="name"
        @change="onSelectView"
      >
        <template #prepend-item>
          <v-list-item
            ripple
            :disabled="!search"
            @mousedown.prevent
            @click="onSelectCreateView"
          >
            <v-list-item-content>
              <v-list-item-title>
                {{ search || "Type to add new column view..." }}
              </v-list-item-title>
              <v-list-item-subtitle>Create column view</v-list-item-subtitle>
            </v-list-item-content>
          </v-list-item>
          <v-divider class="mt-2" />
        </template>
        <template #item="{ item }">
          <v-list-item-content>
            <v-list-item-title>
              {{ item.name }}
            </v-list-item-title>
          </v-list-item-content>
        </template>
      </v-autocomplete>
    </v-col>
    <v-col class="py-1" md="auto">
      <v-btn
        icon
        small
        title="Apply column view"
        :disabled="selectedUuid === null"
        @click="onClickApplyView"
      >
        <v-icon small>
          mdi-clipboard-check-outline
        </v-icon>
      </v-btn>
      <v-dialog v-model="renameDialog" width="640">
        <v-card>
          <v-card-title>
            <span class="text-h5">Rename column view</span>
          </v-card-title>
          <v-card-text>
            <v-text-field v-model="renameName" label="Rename column view" />
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn
              color="blue darken-1"
              text
              @click="renameDialog = false"
            >
              Close
            </v-btn>
            <v-btn
              color="blue darken-1"
              text
              @click="onConfirmRenameView"
            >
              Rename
            </v-btn>
          </v-card-actions>
        </v-card>
        <template #activator="{ on, attrs }">
          <v-btn
            icon
            small
            v-bind="attrs"
            title="Rename column view"
            :disabled="selectedUuid === null"
            v-on="on"
            @click="onClickRenameView"
          >
            <v-icon small>
              mdi-pencil
            </v-icon>
          </v-btn>
        </template>
      </v-dialog>
      <v-dialog v-model="removeDialog" width="640">
        <v-card>
          <v-card-title>
            <span class="text-h5">Delete column view</span>
          </v-card-title>
          <v-card-text>
            Are you should you wish to delete "{{ selectedName }}"?
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn
              color="blue darken-1"
              text
              @click="removeDialog = false"
            >
              Close
            </v-btn>
            <v-btn
              color="blue darken-1"
              text
              @click="onConfirmRemoveView"
            >
              Delete
            </v-btn>
          </v-card-actions>
        </v-card>
        <template #activator="{ on, attrs }">
          <v-btn
            icon
            small
            v-bind="attrs"
            title="Delete column view"
            :disabled="selectedUuid === null"
            v-on="on"
          >
            <v-icon small>
              mdi-delete
            </v-icon>
          </v-btn>
        </template>
      </v-dialog>
      <v-btn
        icon
        small
        title="Save this column view"
        :disabled="selectedUuid === null"
        @click="onClickSaveView"
      >
        <v-icon small>
          mdi-content-save
        </v-icon>
      </v-btn>
    </v-col>
  </v-row>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';

export default {
  name: 'ColumnView',
  props: {
    selectedUuid: {
      type: String,
      default: null,
    },
  },
  data: () => ({
    search: null,
    renameDialog: false,
    renameName: null,
    removeDialog: false,
  }),
  computed: {
    ...mapGetters('columnViews', ['getAllColumnViews', 'getColumnView']),
    selected() {
      if (this.selectedUuid !== null) {
        return this.getColumnView(this.selectedUuid);
      }
      return null;
    },
    selectedName() {
      return this.selected?.name;
    },
  },
  methods: {
    ...mapActions('columnViews', [
      'updateColumnView',
      'removeColumnView',
      'addColumnView',
    ]),
    async onClickSaveView() {
      this.$emit('save-column-view');
    },
    onClickApplyView() {
      this.$emit('apply-column-view');
    },
    async onConfirmRemoveView() {
      await this.removeColumnView(this.selectedUuid);
      this.$emit('update:selectedUuid', null);
      this.removeDialog = false;
    },
    onClickRenameView() {
      this.renameName = this.selectedName;
    },
    async onConfirmRenameView() {
      await this.updateColumnView({
        uuid: this.selectedUuid,
        name: this.renameName,
      });
      this.renameDialog = false;
    },
    onSelectView(uuid) {
      this.$emit('update:selectedUuid', uuid);
    },
    async onSelectCreateView() {
      this.$emit('add-column-view', this.search.trim());
    },
  },
};
</script>
