import { createRouter, createWebHistory } from 'vue-router';
import { SplitView } from '@/views';
import { CreateGlobalPluginView } from '@/views/GlobalPlugins/CreateGlobalPlugin';
import { EditGlobalPluginView } from '@/views/GlobalPlugins/EditGlobalPlugin';
import { ProviderCollection } from './Provider';
import { SettingView } from '@/views/SettingView';
import { GlobalPluginsView } from '@/views/GlobalPlugins';
import ProjectSlagResolver from './Resolver/ProjectSlagResolver.vue';
import ComingSoonView from '@/views/Service/ComingSoonView.vue';
import NotFoundView from '@/views/Service/NotFoundView.vue';
import { LoginView } from '@/views/Auth';
import { useAuth } from 'vue-auth3';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'Login',
      component: LoginView,
    },
    {
      path: '/',
      name: 'Provider',
      component: ProviderCollection,
      beforeEnter: (to, from, next) => {
        const auth = useAuth();
        if (!auth.check()) {
          next({ name: 'Login' });
        } else next();
      },
      children: [
        {
          name: 'ProjectNameResolver',
          path: '/',
          component: ProjectSlagResolver,
          children: [
            {
              path: '/',
              name: 'SplitViewDefault',
              component: SplitView,
            },
            {
              path: '/:projectSlag',
              name: 'SplitView',
              component: SplitView,
            },
          ],
        },
        {
          path: '/settings',
          name: 'settings',
          redirect: '/settings/plugins',
          component: SettingView,
          children: [
            {
              path: '/settings/user-management',
              name: 'users',
              component: ComingSoonView,
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
              path: '/settings/global-logs',
              name: 'global-logs',
              component: ComingSoonView,
            },
          ],
        },
      ],
    },
    { path: '/:pathMatch(.*)*', name: 'NotFound', component: NotFoundView },
  ],
});

export default router;
