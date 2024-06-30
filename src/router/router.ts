import { createRouter, createWebHistory } from 'vue-router';
import { SplitView } from '@/views';
import { ProviderCollection } from './Provider';
import TestConfirmationDialog from '@/views/ConfirmActionView/TestViewConfirmationDialog.vue';

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
          path: '/test-confirmation-dialog',
          name: 'TestConfirmationDialog',
          component: TestConfirmationDialog,
        },
      ],
    },
  ],
});

export default router;
