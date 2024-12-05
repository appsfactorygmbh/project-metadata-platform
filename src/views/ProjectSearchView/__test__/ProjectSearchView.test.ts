import { VueWrapper, flushPromises, mount } from '@vue/test-utils';
import ProjectSearchView from '../ProjectSearchView.vue';
import { describe, expect, it } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { usePluginsStore, useProjectStore } from '@/store';
import { createPinia, setActivePinia } from 'pinia';
import {
  pluginStoreSymbol,
  projectsStoreSymbol,
} from '@/store/injectionSymbols';
import type { ComponentPublicInstance } from 'vue';
import router from '@/router';
import type { ProjectModel } from '@/models/Project';
import { useSearchStore } from '@/store';

interface ProjectSearchViewInstance {
  paneWidth: number;
  paneHeight: number;
  handleRowClick: (project: ProjectModel) => void;
  isFiltering: boolean;
}
const testProject = [
  {
    id: 100,
    projectName: 'Test',
    clientName: 'HTWK',
    businessUnit: '2',
    teamNumber: 10,
  },
];

// Fails with: Cannot use 'in' operator to search for 'addEventListener' in undefined
// TODO: Fix this test. It lets pipeline fail because of jsdom not implementing window.getComputedStyle and other issues
describe('ProjectSearchView.vue', () => {
  setActivePinia(createPinia());
  const searchStoreSymbol = Symbol('searchStoreSym');
  const searchStore = useSearchStore('test');
  const projectsStore = useProjectStore();
  const pluginStore = usePluginsStore();

  const generateWrapper = (pWidth: number) => {
    return mount(ProjectSearchView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        provide: {
          [projectsStoreSymbol as symbol]: projectsStore,
          [pluginStoreSymbol as symbol]: pluginStore,
          [searchStoreSymbol as symbol]: searchStore,
        },
        plugins: [router],
      },
      propsData: {
        paneWidth: pWidth,
        paneHeight: 800,
      },
    });
  };

  it('renders correctly with 4 columns', () => {
    const wrapper = generateWrapper(800);
    expect(wrapper.findAll('.ant-table-column-sorters')).toHaveLength(4);
  });

  it('renders correctly with reset button', async () => {
    const wrapper = generateWrapper(800);
    expect(wrapper.find('.reset').exists()).toBe(true);
  });

  it.skip('hides columns when the pane width is not large enough', async () => {
    createTestingPinia({});
    const wrapper = generateWrapper(300);

    expect(wrapper.findAll('.ant-table-column-sorters').length).toBe(2);
  });

  it('adds a query when clicking on a project', async () => {
    await router.isReady();

    const wrapper = generateWrapper(700) as VueWrapper<
      ComponentPublicInstance<ProjectSearchViewInstance>
    >;
    const testProject = {
      id: 200,
      projectName: 'test',
      clientName: 'test',
      businessUnit: 'test',
      teamNumber: 1,
      isArchived: false,
    };

    wrapper.vm.handleRowClick(testProject);
    await flushPromises();

    expect(Number(router.currentRoute.value.query.projectId)).toBe(
      testProject.id,
    );
  });

  it.skip('requests data with the project id given in the URL', async () => {
    await router.push({
      path: '/',
      query: { projectId: '300' },
    });
    await router.isReady();
    createTestingPinia({});

    const projectStore = useProjectStore();
    const pluginStore = usePluginsStore();
    generateWrapper(800);
    await flushPromises();

    expect(projectStore.fetchProject).toHaveBeenCalledWith(300);
    expect(pluginStore.fetchPlugins).toHaveBeenCalledWith(300);
  });

  it('reset is filter flag when clear filter', async () => {
    searchStore.setBaseSet(testProject);

    searchStore.setSearchQuery('A');
    expect(searchStore.getIsFiltering).toBe(true);
    searchStore.setSearchQuery('');
    expect(searchStore.getIsFiltering).toBe(false);
  });
});
