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
import { UserListView } from '@/views/SettingView/UserManagementView/UserListView';
import { CreateUserView } from '@/views/SettingView/UserManagementView/CreateUser';
import { UserInformationView } from '@/views/SettingView/UserManagementView/UserInformationView';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'Login',
      component: LoginView,
      meta: { title: 'Project Metadata Platform - Login' },
    },
    {
      path: '/register',
      name: 'Register',
      component: RegisterView,
      meta: { title: 'Project Metadata Platform - Register' },
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
          meta: { title: 'Project Metadata Platform - Settings' },
          children: [
            {
              path: '/settings/user-management',
              name: 'usersList',
              component: UserListView,
              meta: { title: 'Project Metadata Platform - User Management' },
              children: [
                {
                  path: '/settings/user-management/user',
                  name: 'usersInformation',
                  component: UserInformationView,
                  meta: {
                    title: 'Project Metadata Platform - User Information',
                  },
                  children: [],
                },
                {
                  path: '/settings/user-management/create',
                  name: 'createUsers',
                  component: CreateUserView,
                  meta: { title: 'Project Metadata Platform - User Creation' },
                },
              ],
            },
            {
              path: '/settings/global-plugins',
              name: 'plugins',
              component: GlobalPluginsView,
              meta: { title: 'Project Metadata Platform - Plugins' },
              children: [
                {
                  path: '/settings/global-plugins/create',
                  name: 'CreateGlobalPlugin',
                  component: CreateGlobalPluginView,
                  meta: { title: 'Project Metadata Platform - Create Plugin' },
                },
                {
                  path: '/settings/global-plugins/edit/',
                  name: 'EditGlobalPlugin',
                  component: EditGlobalPluginView,
                  meta: { title: 'Project Metadata Platform - Edit Plugin' },
                },
              ],
            },
            {
              path: '/settings/global-logs',
              name: 'global-logs',
              component: ComingSoonView,
              meta: { title: 'Project Metadata Platform - Global Logs' },
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
      meta: { title: 'Project Metadata Platform - Not Found' },
    },
    {
      path: '/403',
      name: 'Forbidden',
      component: ForbiddenView,
      meta: { title: 'Project Metadata Platform - Forbidden' },
    },
  ],
});

// Dynamic title changes
if (process.env.NODE_ENV !== 'test') {
  router.afterEach((to) => {
    document.title = (to.meta.title as string) || 'Project Metadata Platform';
  });
}

export default router;
