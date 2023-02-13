<template>
  <div :style="$attrs.style" />
</template>
<script>
import loader from '@monaco-editor/loader';
import loadMonacoKusto from '@/helpers/loadMonacoKusto';

export default {
  name: 'Editor',
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
    language: {
      type: String,
      default: null,
    },
  },
  data: () => ({
    editor: null,
  }),
  watch: {
    value(newValue) {
      if (this.editor && this.value !== this.editor.getValue()) {
        this.editor.setValue(newValue);
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
      await loadMonacoKusto();

      this.editor = monaco.editor.create(
        this.$el,
        {
          value: this.value,
          language: this.language,
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
  },
};
</script>
