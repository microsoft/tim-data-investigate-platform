<template>
  <div>
    <v-combobox
      label="Cluster"
      :value="cluster"
      :items="getClusters"
      :rules="[(v) => !!v || 'Cluster is required']"
      @input="$emit('update:cluster', $event)"
    />
    <v-combobox
      label="Database"
      :value="database"
      :items="getDatabases"
      :hide-no-data="true"
      :rules="[(v) => !!v || 'Database is required']"
      @input="$emit('update:database', $event)"
    />
  </div>
</template>
<script>

import runtimeConfig from '@/helpers/runtimeConfig';

export default {
  name: 'ClusterSelection',
  props: {
    cluster: {
      type: String,
      default: null,
    },
    database: {
      type: String,
      default: null,
    },
  },
  computed: {
    getClusters() {
      const items = [];
      runtimeConfig.defaultClusters.forEach((v) => {
        items.push({
          header: v.name,
        });
        items.push(...v.clusters);
      });
      return items;
    },
    getDatabases() {
      return runtimeConfig.defaultClusters
        .find((clusterGroup) => clusterGroup.clusters.includes(this.cluster))
        ?.databases ?? [];
    },
  },
};
</script>

<style></style>
