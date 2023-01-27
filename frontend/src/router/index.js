import Vue from 'vue';
import VueRouter from 'vue-router';

const OpenTriage = () => import('@/views/OpenTriage.vue');
const QueryEditor = () => import('@/views/QueryEditor.vue');
const Welcome = () => import('@/views/Welcome.vue');
const ShareQuery = () => import('@/views/ShareQuery.vue');
const ExportImport = () => import('@/views/ExportImport.vue');

Vue.use(VueRouter);

const routes = [
  {
    path: '/queries/',
    name: 'QueryEditor',
    component: QueryEditor,
  },
  {
    path: '/view/:uuid',
    name: 'OpenTriage',
    component: OpenTriage,
    props: true,
  },
  {
    path: '/share/:uuid',
    name: 'ShareQuery',
    component: ShareQuery,
    props: true,
  },
  {
    path: '/exportimport/',
    name: 'Export Import',
    component: ExportImport,
  },
  {
    path: '/',
    name: 'Welcome',
    component: Welcome,
  },
];

const router = new VueRouter({
  routes,
});

export default router;
