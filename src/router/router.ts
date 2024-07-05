import { createRouter, createWebHistory } from 'vue-router';
import { SplitView } from '@/views';
import { ProviderCollection } from './Provider';
import { SettingView } from '@/views/SettingView';

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
      ],
    },
    {
      path: '/settings',
      name: 'settings',
      component: SettingView,
      children: [
        {
          path: '/settings/users',
          name: 'users',
          component: SettingView,
        },
        {
          path: '/settings/plugins',
          name: 'plugins',
          component: SettingView,
        },
        {
          path: '/settings/global-logs',
          name: 'global-logs',
          component: SettingView,
        },
      ],
    },
  ],
});

export default router;
