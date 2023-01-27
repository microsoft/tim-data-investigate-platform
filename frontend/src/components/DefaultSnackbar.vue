<template>
  <v-snackbar
    v-model="showSnackbar"
    :timeout="options.timeout"
    bottom
  >
    <v-icon :color="options.color" left>
      {{ options.icon }}
    </v-icon>
    {{ options.message }}
    <template #action="{ attrs }">
      <v-btn
        v-if="options.link"
        text
        color="blue-grey"
        class="text--lighten-3"
        v-bind="attrs"
        :href="options.link"
        target="_blank"
      >
        {{ options.linkText }}
        <v-icon right>
          mdi-open-in-new
        </v-icon>
      </v-btn>
      <v-btn
        text
        :color="options.color"
        v-bind="attrs"
        @click="onCloseSnackbar"
      >
        Dismiss
        <v-icon right>
          mdi-close
        </v-icon>
      </v-btn>
    </template>
  </v-snackbar>
</template>

<script>
import eventBus from '@/helpers/eventBus';

export default {
  name: 'DefaultSnackbar',
  data: () => ({
    showSnackbar: false,
    options: {},
    queue: [],
    handle: null,
    pause: 200,
  }),
  mounted() {
    eventBus.$on('show:snackbar', (attrs) => {
      this.enqueue({
        color: 'accent',
        icon: 'mdi-information',
        message: '',
        timeout: 5000,
        ...attrs,
      });
    });
  },
  methods: {
    clearSnackbarState() {
      if (this.handle) {
        clearTimeout(this.handle);
        this.handle = null;
      }
      this.showSnackbar = false;
    },
    enqueue(snackbar) {
      this.queue.push(snackbar);
      if (this.handle === null) {
        this.showNextSnackbar();
      }
    },
    onCloseSnackbar() {
      this.clearSnackbarState();
      this.handle = setTimeout(() => {
        this.showNextSnackbar();
      }, this.pause);
    },
    showNextSnackbar() {
      this.clearSnackbarState();

      if (this.queue.length === 0) {
        return;
      }

      this.options = this.queue.shift();

      this.handle = setTimeout(() => {
        this.showNextSnackbar();
      }, this.options.timeout + this.pause);

      this.showSnackbar = true;
    },
  },
};
</script>
