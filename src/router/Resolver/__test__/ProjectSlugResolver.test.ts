import { describe, expect, it } from 'vitest';
import initAuth from '@/auth';
import { enableAutoUnmount, flushPromises, mount } from '@vue/test-utils';
import { ProjectSlugResolver } from '..';
import { createTestingPinia } from '@pinia/testing';
import { createRouter, createWebHistory } from 'vue-router';
import { SplitView } from '@/views';
import { ProviderCollection } from '@/router/Provider';
import { setActivePinia } from 'pinia';
import type { ProjectModel } from '@/models/Project';

const testProjects: ProjectModel[] = [
  {
    id: 200,
    projectName: 'test 1',
    clientName: 'test',
    businessUnit: 'test',
    teamNumber: 1,
    isArchived: false,
    slug: 'test-1',
    company: 'test',
    ismsLevel: 'NORMAL',
  },
  {
    id: 300,
    projectName: 'test 2',
    clientName: 'test',
    businessUnit: 'test',
    teamNumber: 1,
    isArchived: false,
    slug: 'test-2',
    company: 'test',
    ismsLevel: 'NORMAL',
  },
];

const piniaOptions: Parameters<typeof createTestingPinia>[0] = {
  stubActions: false,
  initialState: {
    project: {
      projects: testProjects,
      setProjects: vi.fn(),
    },
  },
};

describe('ProjectSlugResolver.vue', () => {
  enableAutoUnmount(afterEach);
  setActivePinia(createTestingPinia(piniaOptions));

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

  const generateWrapper = () => {
    return mount(ProjectSlugResolver, {
      plugins: [createTestingPinia(piniaOptions)],
      global: {
        plugins: [mockRouter, initAuth()],
      },
    });
  };

  it('resolves an initial project id query', async () => {
    await flushPromises();

    await mockRouter.push({
      name: 'ProjectNameResolver',
      query: { projectId: '200' },
    });
    await mockRouter.isReady();

    generateWrapper();

    expect(mockRouter.currentRoute.value.query.projectId).toBe('200');
    await flushPromises();
    expect(mockRouter.currentRoute.value.path).toBe('/test-1');
  });

  it('changes the slug when changing the query', async () => {
    await flushPromises();
    await mockRouter.push({
      name: 'ProjectNameResolver',
      query: { projectId: '200' },
    });
    await mockRouter.isReady();

    generateWrapper();

    expect(mockRouter.currentRoute.value.query.projectId).toBe('200');
    await flushPromises();
    expect(mockRouter.currentRoute.value.path).toBe('/test-1');

    await mockRouter.push({
      name: 'ProjectNameResolver',
      query: { projectId: '300' },
    });
    await mockRouter.isReady();

    expect(mockRouter.currentRoute.value.query.projectId).toBe('300');
    setTimeout(() => {
      expect(mockRouter.currentRoute.value.path).toBe('/test-2');
    }, 1000);
  });
});
