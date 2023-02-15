<template>
  <div :style="$attrs.style" />
</template>
<script>
import loader from '@monaco-editor/loader';
import loadMonacoKusto from '@/helpers/loadMonacoKusto';
import { getKustoSchema } from '@/helpers/apiClient';

export default {
  name: 'KustoMonacoEditor',
  model: {
    event: 'change',
  },
  props: {
    options: {
      default: () => {},
      type: Object,
    },
    value: {
      type: String,
      required: true,
    },
    cluster: {
      type: String,
      default: null,
    },
    database: {
      type: String,
      default: null,
    },
  },
  data: () => ({
    monaco: null,
    editor: null,
  }),
  watch: {
    value(newValue) {
      if (this.editor && this.value !== this.editor.getValue()) {
        this.editor.setValue(newValue);
      }
    },
    cluster(newCluster, oldCluster) {
      if (newCluster && oldCluster !== newCluster) {
        this.loadSchema();
      }
    },
    database(newDatabase, oldDatabase) {
      if (newDatabase && oldDatabase !== newDatabase) {
        this.loadSchema();
      }
    },
  },
  mounted() {
    this.initialiseMonaco();
  },
  methods: {
    async initialiseMonaco() {
      loader.config({
        paths: {
          vs: `${import.meta.env.BASE_URL}monaco-editor/min/vs`,
        },
      });

      const monaco = await loader.init();
      this.monaco = monaco;

      await loadMonacoKusto();
      console.log('Loaded monaco kusto.');

      this.editor = monaco.editor.create(
        this.$el,
        {
          value: this.value,
          language: 'kusto',
          ...this.options,
        },
      );
      this.editor.onDidChangeModelContent(() => {
        const value = this.editor.getValue();
        if (this.value !== value) {
          this.$emit('change', value);
        }
      });
    },
    async loadSchema() {
      if (this.cluster === '' || this.database === '') {
        return;
      }

      const schema = await this.parseSchema();
      const workerAccessor = await this.monaco.languages.kusto.getKustoWorker();
      const model = this.editor.getModel();
      const worker = await workerAccessor(model.uri);
      await worker.setSchemaFromShowSchema(
        schema,
        this.cluster,
        this.database,
      );
    },
    async parseSchema() {
      const schema = await getKustoSchema(this.cluster, this.database);
      return JSON.parse(schema?.data?.at(0)?.ClusterSchema);
    },
  },
};
</script>
