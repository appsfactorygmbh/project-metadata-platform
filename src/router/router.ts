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
import { LoginView, RegisterView } from '@/views/Auth';
import ForbiddenView from '@/views/Service/ForbiddenView.vue';
import UserInformationView from '@/views/UserInformationView/UserInformationView.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'Login',
      component: LoginView,
    },
    {
      path: '/register',
      name: 'Register',
      component: RegisterView,
    },
    {
      path: '/',
      name: 'Provider',
      component: ProviderCollection,
      meta: { auth: true },
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
          redirect: '/settings/global-plugins',
          component: SettingView,
          children: [
            {
              path: '/settings/user-management',
              name: 'users',
              component: UserInformationView,
            },
            {
              path: '/settings/global-plugins',
              name: 'plugins',
              component: GlobalPluginsView,
              children: [
                {
                  path: '/settings/global-plugins/create',
                  name: 'CreateGlobalPlugin',
                  component: CreateGlobalPluginView,
                },
                {
                  path: '/settings/global-plugins/edit/',
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
    {
      name: 'NotFoundRedirect',
      path: '/:pathMatch(.*)*',
      redirect: '/404',
    },
    {
      path: '/404',
      name: 'NotFound',
      component: NotFoundView,
    },
    {
      path: '/403',
      name: 'Forbidden',
      component: ForbiddenView,
    },
  ],
});

export default router;
