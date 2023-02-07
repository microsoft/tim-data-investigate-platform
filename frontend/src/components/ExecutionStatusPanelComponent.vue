<template>
  <div>
    <div class="ag-status-name-value ag-status-panel">
      <span>Execution Time:</span>
      <span class="pl-1 ag-status-name-value-value">{{ executionTime }}</span>
    </div>
    <div v-if="cpuUsage" class="ag-status-name-value ag-status-panel">
      <span>CPU Time:</span>
      <span class="pl-1 ag-status-name-value-value">{{ cpuUsage }}</span>
    </div>
    <div v-if="memoryUsage" class="ag-status-name-value ag-status-panel">
      <span>Memory:</span>
      <span class="pl-1 ag-status-name-value-value">{{ memoryUsage }}MB</span>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ExecutionStatusPanelComponent',
  computed: {
    executionTime() {
      return this.params.context.componentParent.executionTime;
    },
    cpuUsage() {
      return this.params.context.componentParent.cpuUsage;
    },
    memoryUsage() {
      const { memoryUsage } = this.params.context.componentParent;
      if (memoryUsage) {
        return Math.round(memoryUsage / 1024 / 1024);
      }
      return null;
    },
  },
};
</script>
