import { describe, expect, it } from 'vitest';
import auth from '@/auth';
import { enableAutoUnmount, flushPromises, mount } from '@vue/test-utils';
import { ProjectSlugResolver } from '..';
import { createTestingPinia } from '@pinia/testing';
import { createRouter, createWebHistory } from 'vue-router';
import { SplitView } from '@/views';
import { ProviderCollection } from '@/router/Provider';
import { useProjectStore } from '@/store';

const testProjects = [
  {
    id: 200,
    projectName: 'test 1',
    clientName: 'test',
    businessUnit: 'test',
    teamNumber: 1,
    isArchived: false,
  },
  {
    id: 300,
    projectName: 'test 2',
    clientName: 'test',
    businessUnit: 'test',
    teamNumber: 1,
    isArchived: false,
  },
];

// TODO: Fix this test. It lets pipeline fail because of jsdom not implementing window.getComputedStyle and other issues
describe.skip('ProjectSlugResolver.vue', () => {
  enableAutoUnmount(afterEach);
  createTestingPinia();
  const projectStore = useProjectStore();

  const mockRouter = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
      {
        path: '/',
        name: 'Provider',
        component: ProviderCollection,
        children: [
          {
            name: 'ProjectNameResolver',
            path: '/',
            component: ProjectSlugResolver,
            children: [
              {
                path: '/',
                name: 'SplitViewDefault',
                component: SplitView,
              },
              {
                path: '/:projectSlug',
                name: 'SplitView',
                component: SplitView,
              },
            ],
          },
        ],
      },
    ],
  });

  it('resolves an initial project id query', async () => {
    await mockRouter.push({
      name: 'Provider',
      query: { projectId: '200' },
    });
    await mockRouter.isReady();

    mount(ProjectSlugResolver, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        plugins: [mockRouter, auth],
      },
    });

    projectStore.setProjects(testProjects);
    await flushPromises();

    expect(mockRouter.currentRoute.value.query.projectId).toBe('200');
    expect(mockRouter.currentRoute.value.path).toBe('/test-1');
  });

  it('changes the slug when changing the query', async () => {
    await mockRouter.push({
      name: 'Provider',
      query: { projectId: '200' },
    });
    await mockRouter.isReady();

    mount(ProjectSlugResolver, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        plugins: [mockRouter, auth],
      },
    });

    projectStore.setProjects(testProjects);
    await flushPromises();

    expect(mockRouter.currentRoute.value.query.projectId).toBe('200');
    expect(mockRouter.currentRoute.value.path).toBe('/test-1');

    await mockRouter.push({
      name: 'Provider',
      query: { projectId: '300' },
    });
    await flushPromises();

    expect(mockRouter.currentRoute.value.query.projectId).toBe('300');
    expect(mockRouter.currentRoute.value.path).toBe('/test-2');
  });
});
