<template>
  <v-container class="fill-height" fluid>
    <div style="width: 100%; height: 100%; margin-left: 56px">
      <keep-alive :max="100">
        <component
          :is="componentName"
          :key="`${componentName}-${uuid}`"
          :name="`${componentName}-${uuid}`"
          :uuid="uuid"
        />
      </keep-alive>
    </div>
    <DetailSidePanel
      v-if="detailPanelData"
      :data="detailPanelData"
      :is-side-panel-active="showDetailSidePanel"
      @close:side-panel="showDetailSidePanel = false"
    />
    <TaggedEventDialog />
  </v-container>
</template>

<script>
import DetailSidePanel from '@/components/DetailSidePanel.vue';
import eventBus from '@/helpers/eventBus';
import TaggedEventDialog from '@/components/TagEventDialog.vue';
import TemplateQueryResult from '@/components/TemplateQueryResult.vue';
import { mapGetters } from 'vuex';
import KustoQueryResult from '@/components/KustoQueryResult.vue';
import NewQueryButton from '@/components/NewQueryButton.vue';

export default {
  name: 'OpenTriage',
  components: {
    NewQueryButton,
    TemplateQueryResult,
    KustoQueryResult,
    TaggedEventDialog,
    DetailSidePanel,
  },
  props: {
    uuid: {
      type: String,
      required: true,
    },
  },
  data: () => ({
    showDetailSidePanel: false,
    detailPanelData: null,
    debugNode: null,
  }),
  computed: {
    ...mapGetters('displayComponent', ['getComponentName', 'isComponent']),
    componentName() {
      if (this.isComponent(this.uuid)) {
        return this.getComponentName(this.uuid);
      }
      return null;
    },
  },
  beforeMount() {
    eventBus.$on('show:detail-side-panel', this.onShowDetailSidePanel);
    eventBus.$on('update:detail-side-panel', this.onUpdateDetailSidePanel);

    if (!this.isComponent(this.uuid)) {
      this.$router.replace('/');
    }
  },
  methods: {
    onShowDetailSidePanel(data) {
      this.showDetailSidePanel = true;
      this.detailPanelData = data;
    },
    onUpdateDetailSidePanel(data) {
      if (this.showDetailSidePanel) {
        this.detailPanelData = data;
      }
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

.ag-theme-balham .ag-grade-tp {
  background-color: #fbbbb9;
}

.ag-theme-balham .ag-grade-bp {
  background-color: #c2dfff;
}

.ag-theme-balham .ag-grade-fp {
  background-color: #99c68e;
}
</style>
