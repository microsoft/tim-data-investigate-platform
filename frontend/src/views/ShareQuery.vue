<template>
  <v-container v-if="error">
    <v-alert
      border="left"
      type="error"
      text
    >
      {{ error }}
    </v-alert>
  </v-container>
</template>

<script>
import { mapGetters } from 'vuex';
import { createNewTemplateQueryComponent } from '@/helpers/displayComponent';

export default {
  name: 'ShareQuery',
  props: {
    uuid: {
      type: String,
      required: true,
    },
  },
  data: () => ({
    error: null,
  }),
  computed: {
    ...mapGetters('queries', ['getTemplate', 'getQueryTemplates']),
  },
  async mounted() {
    const queryTemplate = this.getTemplate(this.uuid);

    if (!queryTemplate) {
      this.error = 'This query was not found.';
      return;
    }

    if (!this.$route.query.p) {
      this.error = 'Parameters are missing.';
      return;
    }

    const params = {
      ...queryTemplate.getDefaultParams(),
      ...JSON.parse(atob(this.$route.query.p)),
    };
    const title = queryTemplate.buildSummary(params);
    const autoExecuteQuery = this.$route.query.execute === '1';

    await createNewTemplateQueryComponent(
      title,
      queryTemplate,
      params,
      null,
      autoExecuteQuery,
      true,
    );
  },
};
</script>

<style></style>
