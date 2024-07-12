import { createRouter, createWebHistory } from 'vue-router';
import { SplitView } from '@/views';
import { ProviderCollection } from './Provider';
import { SettingView } from '@/views/SettingView';
import { GlobalPluginsView } from '@/views/GlobalPlugins';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'Provider',
      component: ProviderCollection,
      children: [
        {
          path: '/',
          name: 'SplitView',
          component: SplitView,
        },
        {
          path: '/:projectName/',
          name: 'project',
          component: SplitView,
        },
      ],
    },
    {
      path: '/settings',
      name: 'settings',
      redirect: '/settings/global-plugins',
      component: SettingView,
      children: [
        {
          path: '/settings/user-management',
          name: 'users',
          component: GlobalPluginsView,
        },
        {
          path: '/settings/global-plugins',
          name: 'plugins',
          component: GlobalPluginsView,
        },
        {
          path: '/settings/global-logs',
          name: 'global-logs',
          component: GlobalPluginsView,
        },
      ],
    },
  ],
});

export default router;
