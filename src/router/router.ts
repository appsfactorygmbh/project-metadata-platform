import { createRouter, createWebHistory } from 'vue-router';
import { SplitView } from '@/views';
import { CreateGlobalPluginView } from '@/views/GlobalPlugins/CreateGlobalPlugin';
import { EditGlobalPluginView } from '@/views/GlobalPlugins/EditGlobalPlugin';
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
          path: '/settings',
          name: 'settings',
          component: SettingView,
          children: [
            {
              path: '/settings/users',
              name: 'users',
              component: GlobalPluginsView,
            },
            {
              path: '/settings/plugins',
              name: 'plugins',
              component: GlobalPluginsView,
              children: [
                {
                  path: '/settings/plugins/create',
                  name: 'CreateGlobalPlugin',
                  component: CreateGlobalPluginView,
                },
                {
                  path: '/settings/plugins/edit/',
                  name: 'EditGlobalPlugin',
                  component: EditGlobalPluginView,
                },
              ],
            },
            {
              path: '/settings/global_logs',
              name: 'global-logs',
              component: GlobalPluginsView,
            },
          ],
        },
      ],
    },
  ],
});
export default router;
