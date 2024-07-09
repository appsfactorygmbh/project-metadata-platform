import { createRouter, createWebHistory } from 'vue-router';
import { SplitView } from '@/views';
import { CreateGlobalPluginView } from '@/views/GlobalPlugins/CreateGlobalPlugin';
import { AddGlobalPluginView } from '@/views/GlobalPlugins/AddGlobalPlugin';
import { EditGlobalPluginView } from '@/views/GlobalPlugins/EditGlobalPlugin';
import { ProviderCollection } from './Provider';

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
          path: '/settings/plugins/create',
          name: 'CreateGlobalPlugin',
          component: CreateGlobalPluginView,
        },
        {
          path: '/settings/plugins/edit/:pluginId',
          name: 'EditGlobalPlugin',
          component: EditGlobalPluginView,
        },
        {
          path: '/project/plugins/add',
          name: 'AddGlobalPlugin',
          component: AddGlobalPluginView,
        },
      ],
    },
  ],
});

export default router;
