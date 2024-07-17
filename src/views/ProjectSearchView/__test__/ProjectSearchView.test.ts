import { flushPromises, mount, VueWrapper } from '@vue/test-utils';
import ProjectSearchView from '../ProjectSearchView.vue';
import { describe, it, expect } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { usePluginsStore, useProjectStore } from '@/store';
import { createPinia, setActivePinia } from 'pinia';
import _ from 'lodash';
import {
  pluginStoreSymbol,
  projectsStoreSymbol,
} from '@/store/injectionSymbols';
import type { ComponentPublicInstance } from 'vue';
import router from '@/router';
import type { ProjectModel } from '@/models/Project';

interface ProjectSearchViewInstance {
  paneWidth: number;
  paneHeight: number;
  handleRowClick: (project: ProjectModel) => void;
}

// Fails with: Cannot use 'in' operator to search for 'addEventListener' in undefined
describe('ProjectSearchView.vue', () => {
  setActivePinia(createPinia());

  const generateWrapper = (
    pWidth: number,
    projectsStore = useProjectStore(),
    pluginStore = usePluginsStore(),
  ) => {
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
    createTestingPinia({});
    const wrapper = generateWrapper(300);
    expect(wrapper.findAll('.ant-table-column-sorters')).toHaveLength(4);
  });

  it('hides columns when the pane width is not large enough', async () => {
    createTestingPinia({});
    const wrapper = generateWrapper(300);

    _.delay(
      () => expect(wrapper.findAll('.ant-table-column-sorters').length).toBe(2),
      1000,
    );
  });

  it('add a query when clicking on a project', async () => {
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
    };

    wrapper.vm.handleRowClick(testProject);
    await flushPromises();
    expect(Number(router.currentRoute.value.query.projectId)).toBe(
      testProject.id,
    );
  });

  it('requests data with the project id given in the URL', async () => {
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
});
