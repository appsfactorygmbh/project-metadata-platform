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

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'Login',
      component: LoginView,
      meta: { title: 'Metadata Platform - Login' },
    },
    {
      path: '/register',
      name: 'Register',
      component: RegisterView,
      meta: { title: 'Metadata Platform - Register' },
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
          meta: { title: 'Metadata Platform - Settings' },
          children: [
            {
              path: '/settings/user-management',
              name: 'users',
              component: ComingSoonView,
              meta: { title: 'Metadata Platform - User Management' },
            },
            {
              path: '/settings/global-plugins',
              name: 'plugins',
              component: GlobalPluginsView,
              meta: { title: 'Metadata Platform - Plugins' },
              children: [
                {
                  path: '/settings/global-plugins/create',
                  name: 'CreateGlobalPlugin',
                  component: CreateGlobalPluginView,
                  meta: { title: 'Metadata Platform - Create Plugin' },
                },
                {
                  path: '/settings/global-plugins/edit/',
                  name: 'EditGlobalPlugin',
                  component: EditGlobalPluginView,
                  meta: { title: 'Metadata Platform - Edit Plugin' },
                },
              ],
            },
            {
              path: '/settings/global-logs',
              name: 'global-logs',
              component: ComingSoonView,
              meta: { title: 'Metadata Platform - Global Logs' },
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
      meta: { title: 'Metadata Platform - Not Found' },
    },
    {
      path: '/403',
      name: 'Forbidden',
      component: ForbiddenView,
      meta: { title: 'Metadata Platform - Forbidden' },
    },
  ],
});

// After each navigation, this `afterEach` hook dynamically sets the page title
router.afterEach((to) => {
  document.title = to.meta.title || 'Metadata Platform';
});

export default router;
