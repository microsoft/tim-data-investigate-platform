<template>
  <v-app>
    <div>
      <v-toolbar
        dense
        tile
        :elevation="0"
        style="border-bottom: 1px solid rgba(0, 0, 0, 0.1)"
      >
        <v-menu offset-y>
          <template #activator="{ on, attrs }">
            <v-app-bar-nav-icon v-bind="attrs" v-on="on" />
          </template>

          <v-list>
            <v-list-item to="/queries">
              <v-list-item-content class="text-no-wrap">
                Query Manager
              </v-list-item-content>
            </v-list-item>
          </v-list>
        </v-menu>

        <v-toolbar-title>
          <router-link to="/" class="black--text text-decoration-none">
            TIM
          </router-link>
        </v-toolbar-title>

        <v-spacer />

        <v-menu offset-y>
          <template #activator="{ on, attrs }">
            <v-btn
              icon
              v-bind="attrs"
              v-on="on"
            >
              <v-icon>mdi-help</v-icon>
            </v-btn>
          </template>

          <v-list>
            <v-list-item
              :href="`${githubBase}/wiki`"
              target="_blank"
            >
              <v-list-item-title>Wiki Page</v-list-item-title>
              <v-list-item-icon>
                <v-icon>mdi-information</v-icon>
              </v-list-item-icon>
            </v-list-item>

            <v-list-item
              :href="`${githubBase}/issues/new?template=issue_template.md`"
              target="_blank"
            >
              <v-list-item-title>Report a bug</v-list-item-title>
              <v-list-item-icon>
                <v-icon>mdi-bug</v-icon>
              </v-list-item-icon>
            </v-list-item>
          </v-list>
        </v-menu>

        <v-menu offset-y>
          <template #activator="{ on, attrs }">
            <v-btn
              icon
              v-bind="attrs"
              v-on="on"
            >
              <v-icon>mdi-cog-outline</v-icon>
            </v-btn>
          </template>

          <v-list>
            <v-list-item to="/exportimport">
              <v-list-item-content class="text-no-wrap">
                Export / Import
              </v-list-item-content>
            </v-list-item>
          </v-list>
        </v-menu>

        <v-menu offset-y>
          <template #activator="{ on, attrs }">
            <v-btn
              icon
              v-bind="attrs"
              v-on="on"
            >
              <v-icon>mdi-account</v-icon>
            </v-btn>
          </template>

          <v-list>
            <v-list-item v-if="!userId" @click="signIn">
              <v-list-item-content class="text-no-wrap">
                Sign in
              </v-list-item-content>
            </v-list-item>
            <v-list-item v-else @click="onClickLogout">
              <v-list-item-content class="text-no-wrap">
                Sign out
              </v-list-item-content>
            </v-list-item>
          </v-list>
        </v-menu>
      </v-toolbar>
    </div>
    <v-container v-if="!userId">
      <v-alert
        border="left"
        type="info"
        text
      >
        You must <a href="#" @click="signIn">sign-in</a> first.
      </v-alert>
    </v-container>
    <v-main v-else-if="loaded">
      <router-view />
      <SideQueryTree />
    </v-main>
    <v-container v-else>
      <v-alert
        border="left"
        type="info"
        text
      >
        Authenticating and loading queries...
      </v-alert>
    </v-container>
    <v-container v-if="error">
      <v-alert
        border="left"
        type="error"
        text
      >
        {{ error }}
      </v-alert>
    </v-container>
    <DefaultSnackbar />
  </v-app>
</template>

<script>
import auth from '@/helpers/auth';
import SideQueryTree from '@/components/SideQueryTree.vue';
import { mapActions } from 'vuex';
import DefaultSnackbar from '@/components/DefaultSnackbar.vue';
import eventBus from '@/helpers/eventBus';

export default {
  name: 'App',
  components: {
    SideQueryTree,
    DefaultSnackbar,
  },
  data: () => ({
    userId: null,
    error: null,
    loaded: false,
    changeLogBar: true,
    logout: false,
  }),
  computed: {
    githubBase: () => import.meta.env.VITE_GITHUB_BASE,
  },
  mounted() {
    this.runSetup();
    this.draggableEvents();
  },
  methods: {
    ...mapActions('queries', ['loadQueries']),
    ...mapActions('columnViews', ['loadAllColumnViews']),
    async onClickLogout() {
      await auth.logout();
      eventBus.$emit('show:snackbar', {
        message: 'You have successfully logged out.',
      });
      this.logout = true;
      this.userId = null;
    },
    async signIn() {
      await auth.login();
      this.userId = auth.getUserId();

      if (!this.userId) {
        this.error = 'You must login to access TIM. Check that your browser allows pop-up windows for this site.';
        return false;
      }
      return true;
    },
    async runSetup() {
      if (!(await this.signIn())) {
        return;
      }

      /*
      setTimeout(() => {
        eventBus.$emit('show:snackbar', {
          message: 'YYYY-MM-DD: New features',
          icon: 'mdi-new-box',
          link: `${this.githubBase}/blob/main/frontend/CHANGELOG.md`,
          linkText: 'wiki',
          timeout: 10000,
        });
      }, 1000);
      */

      await this.loadQueries();
      await this.loadAllColumnViews();

      this.loaded = true;
    },
    draggableEvents() {
      // Code taken from: https://github.com/vuetifyjs/vuetify/issues/4058#issuecomment-450636420
      const d = {};
      document.addEventListener('mousedown', (e) => {
        const closestDialog = e.target.closest('.v-dialog.v-dialog--active');
        if (e.button === 0 && closestDialog != null && e.target.classList.contains('v-card__title')) { // element which can be used to move element
          d.el = closestDialog; // element which should be moved
          d.mouseStartX = e.clientX;
          d.mouseStartY = e.clientY;
          d.elStartX = d.el.getBoundingClientRect().left;
          d.elStartY = d.el.getBoundingClientRect().top;
          d.el.style.position = 'fixed';
          d.el.style.margin = '0';
          d.oldTransition = d.el.style.transition;
          d.el.style.transition = 'none';
        }
      });
      document.addEventListener('mousemove', (e) => {
        if (d.el === undefined) return;
        d.el.style.left = `${Math.min(
          Math.max(d.elStartX + e.clientX - d.mouseStartX, 0),
          window.innerWidth - d.el.getBoundingClientRect().width,
        )}px`;
        d.el.style.top = `${Math.min(
          Math.max(d.elStartY + e.clientY - d.mouseStartY, 0),
          window.innerHeight - d.el.getBoundingClientRect().height,
        )}px`;
      });
      document.addEventListener('mouseup', () => {
        if (d.el === undefined) return;
        d.el.style.transition = d.oldTransition;
        d.el = undefined;
      });
      setInterval(() => { // prevent out of bounds
        const dialog = document.querySelector('.v-dialog.v-dialog--active');
        if (dialog === null) return;
        dialog.style.left = `${Math.min(parseInt(dialog.style.left, 10), window.innerWidth - dialog.getBoundingClientRect().width)}px`;
        dialog.style.top = `${Math.min(parseInt(dialog.style.top, 10), window.innerHeight - dialog.getBoundingClientRect().height)}px`;
      }, 100);
    },
  },
};
</script>

<style lang="scss">
@import "../node_modules/ag-grid-community/styles/ag-grid.css";
@import "../node_modules/ag-grid-community/styles/ag-theme-balham.css";
</style>

<style>
.v-dialog.v-dialog--active .popup-header {
  cursor: grab;
}

.v-dialog.v-dialog--active .popup-header:active {
  cursor: grabbing;
}
</style>
