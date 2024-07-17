import { describe, it, expect } from 'vitest';
import router from '@/router/router';
import { enableAutoUnmount, flushPromises, mount } from '@vue/test-utils';
import { ProjectSlagResolver } from '..';
import { useProjectStore } from '@/store';
import { createTestingPinia } from '@pinia/testing';
import { projectsStoreSymbol } from '@/store/injectionSymbols';

const testProjects = [
  {
    id: 200,
    projectName: 'test 1',
    clientName: 'test',
    businessUnit: 'test',
    teamNumber: 1,
  },
  {
    id: 300,
    projectName: 'test 2',
    clientName: 'test',
    businessUnit: 'test',
    teamNumber: 1,
  },
];

describe('ProjectSlagResolver.vue', () => {
  enableAutoUnmount(afterEach);
  createTestingPinia();

  it('resolves an initial project id query', async () => {
    await router.push({
      name: 'Provider',
      query: { projectId: '200' },
    });
    await router.isReady();

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
        plugins: [router],
      },
    });
    const projectsStore = useProjectStore();

    projectsStore.setProjects(testProjects);
    await flushPromises();

    expect(router.currentRoute.value.query.projectId).toBe('200');
    expect(router.currentRoute.value.path).toBe('/test-1');
  });

  it('changes the slag when changing the query', async () => {
    await router.push({
      name: 'Provider',
      query: { projectId: '200' },
    });
    await router.isReady();

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
        plugins: [router],
      },
    });
    const projectsStore = useProjectStore();

    projectsStore.setProjects(testProjects);
    await flushPromises();

    expect(router.currentRoute.value.query.projectId).toBe('200');
    expect(router.currentRoute.value.path).toBe('/test-1');

    await router.push({
      name: 'Provider',
      query: { projectId: '300' },
    });
    await flushPromises();

    expect(router.currentRoute.value.query.projectId).toBe('300');
    expect(router.currentRoute.value.path).toBe('/test-2');
  });
});
