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
import clusterGroups from '@/config/kustoClusters';

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
      clusterGroups.forEach((v) => {
        items.push({
          header: v.name,
        });
        items.push(...v.clusters);
      });
      return items;
    },
    getDatabases() {
      return clusterGroups
        .find((clusterGroup) => clusterGroup.clusters.includes(this.cluster))
        ?.databases ?? [];
    },
  },
};
</script>

<style></style>
