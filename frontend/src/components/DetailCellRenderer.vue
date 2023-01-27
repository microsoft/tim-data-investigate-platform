<template>
  <div class="detail-cell d-flex flex-column">
    <div v-for="(value, name) in params.value" :key="name">
      <span class="detail-header">{{ name }}</span>
      <span class="pl-1">{{ value }}</span>
    </div>
  </div>
</template>

<script>
export default {
  name: 'DetailCellRenderer',
  props: {
    params: {
      type: Object,
      required: true,
    },
  },
};
</script>

<style>
.detail-cell .detail-header {
  font-weight: bold;
}
</style>
