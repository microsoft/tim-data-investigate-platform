<template>
  <v-container fluid>
    <v-row class="py-0">
      <v-col cols="7" class="py-1">
        <v-text-field
          v-model="quickFilterText"
          dense
          label="Quick UI filter"
          prepend-inner-icon="mdi-magnify"
          @input="onQuickFilter"
        />
      </v-col>
      <v-col cols="5">
        <ColumnView
          :selected-uuid.sync="selectedColumnViewUuid"
          @apply-column-view="onApplyColumnView"
          @save-column-view="onSaveColumnView"
          @add-column-view="onAddColumnView"
        />
      </v-col>
    </v-row>
    <v-row no-gutters>
      <v-col cols="12">
        <ag-grid-vue
          v-if="rowData"
          class="ag-theme-balham"
          style="height: calc(100vh - 200px)"
          :agg-funcs="aggFuncs"
          :animate-rows="false"
          :cache-quick-filter="true"
          :column-defs="columnDefs"
          :context="context"
          :default-col-def="defaultColDef"
          :enable-range-selection="true"
          :get-context-menu-items="getContextMenuItems"
          :get-row-id="getRowId"
          :grid-options="gridOptions"
          :group-display-type="groupDisplayType"
          :group-row-renderer-params="groupRowRendererParams"
          :group-selects-children="true"
          :maintain-column-order="true"
          :read-only-edit="true"
          :row-buffer="20"
          :row-class-rules="rowClassRules"
          :row-data="rowData"
          :row-selection="'multiple'"
          :side-bar="sideBar"
          :skip-header-on-auto-size="true"
          :status-bar="statusBar"
          :suppress-column-move-animation="false"
          :suppress-field-dot-notation="false"
          :suppress-row-click-selection="true"
          @grid-ready="onGridReady"
          @cell-edit-request="onCellEditRequest"
          @cell-focused="onCellFocused"
        />
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import { AgGridVue } from 'ag-grid-vue';
import eventBus from '@/helpers/eventBus';
import { createMenuContext } from '@/helpers/aghelper';
import { mapActions, mapGetters } from 'vuex';
import ExecutionStatusPanelComponent from '@/components/ExecutionStatusPanelComponent.vue';
import DetailCellRenderer from '@/components/DetailCellRenderer.vue';
import { loadDataStore } from '@/helpers/localStorage';
import { createComments, saveEvents } from '@/helpers/apiClient';
import ColumnView from '@/components/ColumnView.vue';

const checkboxColDef = () => ({
  headerCheckboxSelection: true,
  headerCheckboxSelectionFilteredOnly: true,
  checkboxSelection: true,
  pinned: 'left',
  width: 42,
  resizable: false,
  sortable: false,
  lockPinned: true,
  lockVisible: true,
  lockPosition: true,
  filter: false,
});

export default {
  name: 'KustoPivot',
  components: {
    ColumnView,
    AgGridVue,
    // eslint-disable-next-line vue/no-unused-components
    DetailCellRenderer,
    // eslint-disable-next-line vue/no-unused-components
    ExecutionStatusPanelComponent,
  },
  props: {
    uuid: {
      type: String,
      required: true,
    },
  },
  data: () => ({
    aggFuncs: null,
    columnDefs: null,
    contentMenuItems: null,
    context: null,
    debugNode: null,
    defaultColDef: null,
    getRowId: null,
    gridApi: null,
    gridColumnApi: null,
    gridOptions: null,
    groupDisplayType: null,
    groupRowRendererParams: null,
    quickFilterText: null,
    rowClassRules: null,
    rowData: null,
    savedState: null,
    searchText: null,
    selectedColumnViewUuid: null,
    sideBar: null,
    statusBar: null,
  }),
  computed: {
    ...mapGetters('displayComponent', [
      'getComponentParams',
      'getComponentState',
      'getComponentRowDataTrigger',
    ]),
    ...mapGetters('queries', [
      'getQueryTemplates',
      'getQueryOption',
    ]),
    ...mapGetters('columnViews', ['getColumnView']),
    rowDataTrigger() {
      return this.getComponentRowDataTrigger(this.uuid);
    },
    getColumnId() {
      return this.getComponentParams(this.uuid).queryTemplate?.columnId;
    },
    getColumns() {
      return this.getComponentParams(this.uuid).queryTemplate?.columns || {};
    },
    executionTime() {
      return this.getComponentState(this.uuid).executionTime;
    },
    cpuUsage() {
      return this.getComponentState(this.uuid).cpuUsage;
    },
    memoryUsage() {
      return this.getComponentState(this.uuid).memoryUsage;
    },
    getColumnViewState() {
      if (this.selectedColumnViewUuid !== null) {
        return this.getColumnView(this.selectedColumnViewUuid).columnState;
      }
      return {};
    },
  },
  watch: {
    rowDataTrigger() {
      this.loadRowData();
    },
    getQueryTemplates() {
      this.buildContextMenu();
    },
  },
  activated() {
    if (this.savedState !== null) {
      this.gridColumnApi.applyColumnState({
        state: this.savedState,
        applyOrder: true,
      });
    }
  },
  deactivated() {
    this.savedState = this.gridColumnApi.getColumnState();
  },
  beforeMount() {
    this.gridOptions = {};
    this.rowClassRules = {
      'ag-tag-malicious': (params) => params.data?.TagEvent?.Determination === 'malicious',
      'ag-tag-benign': (params) => params.data?.TagEvent?.Determination === 'benign',
      'ag-tag-suspicious': (params) => params.data?.TagEvent?.Determination === 'suspicious',
    };

    this.columnDefs = [];

    this.groupDisplayType = 'groupRows';
    this.groupRowRendererParams = {
      checkbox: true,
    };

    this.aggFuncs = {
      dcount: (params) => [
        ...new Set(
          params.values
            .flatMap((e) => e)
            .filter((e) => e !== null && e !== ''),
        ),
      ].length,
    };

    this.defaultColDef = {
      enableValue: true,
      editable: true,
      filter: 'agMultiColumnFilter',
      filterParams: {
        filters: [
          {
            filter: 'agTextColumnFilter',
            display: 'subMenu',
          },
          {
            filter: 'agNumberColumnFilter',
            display: 'subMenu',
          },
          {
            filter: 'agDateColumnFilter',
            display: 'subMenu',
            filterParams: {
              comparator: (filterDate, cellValue) => {
                if (cellValue == null) return -1;
                return Date.parse(cellValue) - Date.parse(filterDate);
              },
            },
          },
          {
            filter: 'agSetColumnFilter',
          },
        ],
      },
      enableRowGroup: true,
      sortable: true,
      resizable: true,
      quickFilterText: null,
      cellEditorPopup: true,
      cellEditorPopupPosition: 'under',
      cellEditor: 'agLargeTextCellEditor',
      cellEditorParams: { maxLength: '300', cols: '100', rows: '6' },
      getQuickFilterText: (params) => {
        const value = params.value ?? '';

        if (typeof value === 'object') {
          return JSON.stringify(value);
        }

        return value.toString();
      },
    };

    // eslint-disable-next-line no-underscore-dangle
    this.getRowId = (params) => params.data._id;

    this.sideBar = ['columns', 'filters'];
    this.statusBar = {
      statusPanels: [
        {
          statusPanel: 'ExecutionStatusPanelComponent',
          align: 'left',
        },
        { statusPanel: 'agTotalRowCountComponent' },
        { statusPanel: 'agFilteredRowCountComponent' },
        { statusPanel: 'agSelectedRowCountComponent' },
      ],
    };
    this.context = { componentParent: this };
    this.rowData = [];
  },
  mounted() {
    this.gridApi = this.gridOptions.api;
    this.gridColumnApi = this.gridOptions.columnApi;

    this.loadRowData();
  },
  methods: {
    ...mapActions('columnViews', [
      'updateColumnView',
      'removeColumnView',
      'addColumnView',
    ]),
    async onApplyColumnView() {
      this.gridColumnApi.applyColumnState({
        state: this.getColumnViewState,
        applyOrder: true,
      });
    },
    async onSaveColumnView() {
      await this.updateColumnView({
        uuid: this.selectedColumnViewUuid,
        columnState: this.gridColumnApi.getColumnState(),
      });
    },
    async onAddColumnView(name) {
      this.selectedColumnViewUuid = await this.addColumnView({
        name: name.trim(),
        columnState: this.gridColumnApi.getColumnState(),
      });
    },
    async onCellEditRequest(event) {
      if (event.column?.colId !== 'TagEvent.Comment') {
        return;
      }

      if (event.data?.TagEvent?.IsSaved !== true) {
        return;
      }

      if (!event.data?.EventId || !event.data?.TagEvent?.Determination) {
        return;
      }

      if (event.newValue === event.oldValue) {
        return;
      }

      const comment = event.newValue;

      console.log('Saving comment...');
      eventBus.$emit('show:snackbar', {
        message: 'Quick saving comment...',
      });

      try {
        await createComments([
          {
            eventId: event.data.EventId,
            determination: event.data.TagEvent.Determination,
            comment,
          },
        ]);
      } catch (err) {
        if (err?.response?.status === 400) {
          eventBus.$emit('show:snackbar', {
            message: `Failed to saved events: ${err?.response?.data?.title}`,
            color: 'error',
            icon: 'mdi-alert',
          });
          throw err?.response?.data?.errors;
        }
        throw err;
      }

      eventBus.$emit('show:snackbar', {
        message: 'Comment successfully quick saved.',
        color: 'success',
        icon: 'mdi-check',
      });

      const row = {
        ...(event.data || {}),
        TagEvent: {
          ...(event.data?.TagEvent || {}),
          Comment: comment,
        },
      };
      this.gridApi.applyTransaction({ update: [row] });
    },
    async loadRowData() {
      const newRowData = await loadDataStore(this.uuid);

      if (!newRowData) {
        throw new Error('Failed to load data store.');
      }

      const idSet = new Set();

      this.rowData = Object.freeze(
        newRowData.map((row, index) => {
          const colId = this.getColumnId || 'EventId';
          let id = row[colId];
          if (id === '' || id === undefined || id === null || idSet.has(id)) {
            id = `row-index-${index}`;
          }
          idSet.add(id);

          return {
            ...row,
            _id: id,
          };
        }),
      );
      this.setupColumns();
    },
    async buildContextMenu() {
      this.contentMenuItems = [
        this.customiseTagEventsMenuItem(),
        ...this.quickTagSelectedEventsMenuItem(),
        this.showDetailsMenuItem(),
        'separator',
        ...Object.values(this.getQueryTemplates)
          .filter((queryTemplate) => this.getQueryOption(queryTemplate.uuid).hide !== true)
          .map((queryTemplate) => this.generateMenuItem(queryTemplate))
          .sort((a, b) => {
            if (a.path.toString() === b.path.toString()) {
              return a.name.localeCompare(b.name);
            }
            return a.path.toString().localeCompare(b.path.toString());
          }),
        'separator',
        'copy',
        'copyWithHeaders',
        'export',
      ];
    },
    getContextMenuItems(params) {
      return createMenuContext(this.contentMenuItems, params);
    },
    async onGridReady() {
      await this.buildContextMenu();
      this.setupColumns();
    },
    onQuickFilter() {
      this.gridApi.setQuickFilter(this.quickFilterText);
    },
    customiseTagEventsMenuItem() {
      return {
        name: 'Customise tag events',
        path: ['Tag Events'],
        action: async (params) => {
          const rows = this.getMultidataFromSelected(params.node.data);

          eventBus.$emit('create:tag-event-dialog', {
            events: rows,
            onSuccess: () => {
              this.gridApi.applyTransaction({ update: rows });
              this.gridApi.deselectAll();
            },
          });
        },
        condition: (params) => {
          const rows = this.getMultidataFromSelected(params.node.data);
          return rows.every((data) => data.EventId && data.EventTime);
        },
      };
    },
    quickTagSelectedEventsMenuItem() {
      return ['Malicious', 'Suspicious', 'Benign'].map((determination) => ({
        name: `Quick - ${determination}`,
        path: ['Tag Events'],
        action: async (params) => {
          const rows = this.getMultidataFromSelected(params.node.data);

          console.log('Saving events...');
          eventBus.$emit('show:snackbar', {
            message: 'Quick saving events...',
          });

          const events = rows
            .filter((data) => data.TagEvent?.IsSaved !== true)
            .map((data) => ({
              eventId: data.EventId,
              eventTime: data.EventTime,
              data,
            }));

          if (events.length > 0) {
            try {
              await saveEvents(events);
            } catch (err) {
              if (err?.response?.status === 400) {
                eventBus.$emit('show:snackbar', {
                  message: `Saving events failed: ${err?.response?.data?.title}`,
                  color: 'error',
                  icon: 'mdi-alert',
                });
                throw err?.response?.data?.errors;
              }
              throw err;
            }
          }

          console.log('Saving comments...');
          const comments = rows.map((data) => ({
            eventId: data.EventId,
            determination: determination.toLowerCase(),
          }));
          try {
            await createComments(comments);
          } catch (err) {
            if (err?.response?.status === 400) {
              eventBus.$emit('show:snackbar', {
                message: `Saving comments failed: ${err?.response?.data?.title}`,
                color: 'error',
                icon: 'mdi-alert',
              });
              throw err?.response?.data?.errors;
            }
            throw err;
          }

          eventBus.$emit('show:snackbar', {
            message: 'Tag events successfully saved.',
            color: 'success',
            icon: 'mdi-check',
          });

          rows.forEach((data) => {
            // eslint-disable-next-line no-param-reassign
            data.TagEvent = {
              ...data.TagEvent,
              Determination: determination.toLowerCase(),
              IsSaved: true,
            };
          });
          this.gridApi.applyTransaction({ update: rows });
          this.gridApi.deselectAll();
        },
        condition: (params) => {
          const rows = this.getMultidataFromSelected(params.node.data);
          return rows.every((data) => data.EventId && data.EventTime);
        },
      }));
    },
    showDetailsMenuItem() {
      return {
        name: 'Show details',
        action: (params) => {
          eventBus.$emit('show:detail-side-panel', params.node.data);
        },
      };
    },
    onCellFocused(params) {
      const rowNode = params.api.getDisplayedRowAtIndex(params.rowIndex);
      if (rowNode) {
        eventBus.$emit('update:detail-side-panel', rowNode.data);
      }
    },
    getMultidataFromSelected(data) {
      const selectedNodes = this.gridApi.getSelectedNodes() || [];
      const multipleData = selectedNodes.map((e) => e.data);
      if (multipleData.length === 0 && data !== undefined) {
        multipleData.push(data);
      }
      return multipleData;
    },
    generateMenuItem(queryTemplate) {
      return {
        name: queryTemplate.menu,
        action: (params) => {
          const { data } = params.node;
          const newParams = queryTemplate.buildParams(
            data,
            this.getMultidataFromSelected(data),
          );

          this.$emit('create:query-template', {
            title: queryTemplate.buildSummary(newParams),
            queryTemplate,
            params: newParams,
          });
        },
        path: queryTemplate.path,
      };
    },
    setupColumns() {
      if (!this.rowData || this.rowData.length === 0) {
        return;
      }

      const ignoreColumns = ['_id'];

      this.columnDefs = [
        checkboxColDef(),
        ...Object.entries(this.getColumns)
          .filter(([colName]) => colName !== 'default')
          .map(([colName, colConfig]) => ({
            field: colName,
            headerName: colName,
            ...colConfig,
          })),
        ...Object.keys(this.rowData[0])
          .filter(
            (colName) => !ignoreColumns.includes(colName) && !(colName in this.getColumns),
          )
          .map((colName) => {
            const colConfig = this.getColumns.default || {};

            return {
              field: colName,
              headerName: colName,
              ...colConfig,
            };
          }),
        ...['TagEvent.Tags', 'TagEvent.Comment', 'TagEvent.Determination']
          .filter((colName) => !(colName in this.getColumns))
          .map((colName) => ({
            field: colName,
            headerName: colName,
            hide: true,
          })),
      ];
    },
  },
};
</script>

<style>
.ag-theme-balham .ag-details-row {
  padding: 0;
}
.ag-theme-balham .ag-root-wrapper {
  width: 100%;
}
.ag-body-viewport {
  overflow-y: scroll !important;
}

.ag-theme-balham .ag-tag-malicious {
  background-color: #fbbbb9 !important;
}

.ag-theme-balham .ag-tag-suspicious {
  background-color: #ffecae !important;
}

.ag-theme-balham .ag-tag-benign {
  background-color: #d4f3cd !important;
}

.ag-theme-balham .ag-text-area-input {
  font-family: monospace;
}
</style>
