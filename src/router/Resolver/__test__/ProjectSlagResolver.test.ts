import { describe, expect, it } from 'vitest';
import auth from '@/auth';
import { enableAutoUnmount, flushPromises, mount } from '@vue/test-utils';
import { ProjectSlagResolver } from '..';
import { useProjectStore } from '@/store';
import { createTestingPinia } from '@pinia/testing';
import { projectsStoreSymbol } from '@/store/injectionSymbols';
import { createRouter, createWebHistory } from 'vue-router';
import { SplitView } from '@/views';
import { ProviderCollection } from '@/router/Provider';

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

describe('ProjectSlagResolver.vue', () => {
  enableAutoUnmount(afterEach);
  createTestingPinia();

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

    mount(ProjectSlagResolver, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        provide: {
          [projectsStoreSymbol as symbol]: useProjectStore(),
        },
        plugins: [mockRouter, auth],
      },
    });

    const projectsStore = useProjectStore();
    projectsStore.setProjects(testProjects);
    await flushPromises();

    expect(mockRouter.currentRoute.value.query.projectId).toBe('200');
    expect(mockRouter.currentRoute.value.path).toBe('/test-1');
  });

  it('changes the slag when changing the query', async () => {
    await mockRouter.push({
      name: 'Provider',
      query: { projectId: '200' },
    });
    await mockRouter.isReady();

    mount(ProjectSlagResolver, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        provide: {
          [projectsStoreSymbol as symbol]: useProjectStore(),
        },
        plugins: [mockRouter, auth],
      },
    });

    const projectsStore = useProjectStore();
    projectsStore.setProjects(testProjects);
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
