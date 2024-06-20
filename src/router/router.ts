import { createRouter, createWebHistory } from 'vue-router';
import { SplitView } from '@/views';
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
      ],
    },
  ],
});

export default router;
