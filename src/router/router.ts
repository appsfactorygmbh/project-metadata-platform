import { createRouter, createWebHistory } from 'vue-router';
import SplitView from '@/views/SplitView';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'SplitView',
      component: SplitView,
    },
  ],
});

export default router;
