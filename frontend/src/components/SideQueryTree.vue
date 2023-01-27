<template>
  <v-navigation-drawer
    absolute
    permanent
    expand-on-hover
    width="700"
    :mini-variant.sync="isNavClosed"
  >
    <div
      class="pl-2"
      style="max-width: 100%; min-width: 700px; white-space: nowrap"
    >
      <NewQueryButton
        text
        icon
        tile
      >
        <v-icon>mdi-plus</v-icon>
      </NewQueryButton>
      <v-divider vertical style="height: 18px" />
      <v-btn
        text
        icon
        tile
        title="Reload templates"
        @click="onClickReloadTemplates"
      >
        <v-icon>mdi-refresh</v-icon>
      </v-btn>
      <v-divider vertical style="height: 18px" />
      <v-btn
        :disabled="tree.length === 0"
        text
        icon
        tile
        title="Remove selected"
        @click="onClickRemoveSelected"
      >
        <v-icon>mdi-delete</v-icon>
      </v-btn>
    </div>
    <v-divider />
    <v-treeview-independant-parent
      ref="componentTreeView"
      v-model="tree"
      :items="getRootDisplayComponents"
      :class="isNavClosed ? 'closed-tree' : ''"
      :active="active"
      item-key="componentUuid"
      item-text="title"
      active-class="v-treeview-node--active active-query"
      selection-type="leaf"
      open-all
      activatable
      hoverable
      dense
      selectable
      @update:active="updateActive"
    >
      <template #prepend="{ item, open, active }">
        <v-progress-circular
          v-if="item.state.isExecuting"
          indeterminate
          color="primary"
          :size="16"
          :width="2"
        />
        <v-icon v-else-if="item.state.error" color="error">
          mdi-alert
        </v-icon>
        <v-badge
          v-else-if="item.state.rowCount !== null && item.state.rowCount >= 0"
          :color="
            item.state.isVisited
              ? 'grey'
              : item.state.rowCount === 0
                ? 'warning'
                : 'info'
          "
          :content="
            item.state.rowCount > 9 ? '9+' : item.state.rowCount.toString()
          "
          overlap
        >
          <v-icon :class="active ? 'primary--text' : ''">
            {{ open ? "mdi-folder-open" : "mdi-folder" }}
          </v-icon>
        </v-badge>
        <template v-else>
          <v-icon style="opacity: 0.5" :class="active ? 'primary--text' : ''">
            {{ open ? "mdi-folder-open" : "mdi-folder" }}
          </v-icon>
        </template>
      </template>
      <template #label="{ item }">
        <span>{{ isDraft(item) ? "[draft] " : isNew(item) ? "[new] " : "" }}</span>
        <span :class="isNew(item) ? 'font-weight-medium' : ''">
          {{ item.title }}</span>
      </template>
    </v-treeview-independant-parent>
  </v-navigation-drawer>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import eventBus from '@/helpers/eventBus';
import {
  createNewQueryComponent,
  createNewTemplateQueryComponent,
} from '@/helpers/displayComponent';
import NewQueryButton from '@/components/NewQueryButton.vue';
import VTreeviewIndependantParent from '@/components/VTreeviewIndependantParent.vue';

export default {
  name: 'SideQueryTree',
  components: {
    VTreeviewIndependantParent,
    NewQueryButton,
  },
  data() {
    return {
      isNavClosed: true,
      activeUuid: null,
      tree: [],
      prevTree: [],
    };
  },
  computed: {
    ...mapGetters('displayComponent', [
      'getRootDisplayComponents',
      'getParentDisplayComponent',
      'isComponent',
    ]),
    active() {
      return [this.activeUuid];
    },
    isNew: () => (item) => (
      item.state.rowCount !== null
      && !item.state.isVisited
      && item.state.error === null
    ),
    isDraft: () => (item) => (
      item.state.rowCount === null
      && !item.state.isExecuting
      && item.state.error === null
    ),
  },
  watch: {
    $route(to, from) {
      if (from.params.uuid !== to.params.uuid) {
        this.activeUuid = to.params.uuid;
      }
    },
  },
  mounted() {
    eventBus.$on('new:display-component', ({ uuid }) => {
      const parent = this.getParentDisplayComponent(uuid);
      if (parent) {
        this.$refs.componentTreeView.updateOpen(parent, true);
      }
    });
    eventBus.$on('update:kusto-results', ({ uuid }) => {
      // Results were complete on the active component
      if (uuid === this.activeUuid) {
        this.updateComponentState({ uuid, isVisited: true });
      }
    });
    this.loadAllDisplayComponents();
  },
  methods: {
    ...mapActions('displayComponent', [
      'removeAllDisplayComponents',
      'updateComponentState',
      'loadAllDisplayComponents',
    ]),
    ...mapActions('queries', ['reloadQueries']),
    updateActive(newValue) {
      if (newValue.length === 1) {
        const uuid = newValue[0];
        this.updateComponentState({ uuid, isVisited: true });
        if (uuid !== this.activeUuid) {
          this.$router.push({ name: 'OpenTriage', params: { uuid } });
        }
      }
    },
    onClickRemoveSelected() {
      this.removeAllDisplayComponents({ uuidArray: this.tree });
      if (this.activeUuid && !this.isComponent(this.activeUuid)) {
        this.$router.replace('/');
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
        true,
      );
    },
    async onClickReloadTemplates() {
      await this.reloadQueries();
    },
  },
};
</script>

<style>
.closed-tree {
}

.closed-tree .v-treeview-node__level,
.closed-tree .v-treeview-node__toggle--open,
.closed-tree .v-treeview-node__label,
.closed-tree .v-treeview-node__toggle,
.closed-tree .v-treeview-node__checkbox {
  display: none;
}

.v-treeview-node__label {
  cursor: pointer;
  white-space: break-spaces;
}

.v-list-item--dense,
.v-list--dense .v-list-item {
  min-height: 50px;
}

.v-treeview--dense .v-treeview-node__label {
  font-size: 0.8125rem;
  line-height: 1rem;
}

.active-query .v-treeview-node__label {
  pointer-events: none;
}
</style>
