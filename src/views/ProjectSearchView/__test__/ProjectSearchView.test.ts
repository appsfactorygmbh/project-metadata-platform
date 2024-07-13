import { flushPromises, mount, VueWrapper } from '@vue/test-utils';
import ProjectSearchView from '../ProjectSearchView.vue';
import { describe, it, expect } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import {
  usePluginsStore,
  useProjectStore,
  type PluginsStore,
  type ProjectStore,
} from '@/store';
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

describe('ProjectSearchView.vue', () => {
  setActivePinia(createPinia());

  const generateWrapper = (
    pWidth: number,
    projectsStore: ProjectStore,
    pluginStore: PluginsStore,
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
    const projectsStore = useProjectStore();
    const pluginStore = usePluginsStore();
    const wrapper = generateWrapper(800, projectsStore, pluginStore);

    expect(wrapper.findAll('.ant-table-column-sorters')).toHaveLength(4);
  });

  it('hides columns when the pane width is not large enough', async () => {
    createTestingPinia({});
    const projectsStore = useProjectStore();
    const pluginStore = usePluginsStore();
    const wrapper = generateWrapper(300, projectsStore, pluginStore);

    _.delay(
      () => expect(wrapper.findAll('.ant-table-column-sorters').length).toBe(2),
      1000,
    );
  });

  it('add a query when clicking on a project', async () => {
    await router.isReady();

    const projectsStore = useProjectStore();
    const pluginStore = usePluginsStore();
    const wrapper = generateWrapper(
      800,
      projectsStore,
      pluginStore,
    ) as VueWrapper<ComponentPublicInstance<ProjectSearchViewInstance>>;

    const testProject = {
      id: 200,
      projectName: 'test',
      clientName: 'test',
      businessUnit: 'test',
      teamNumber: 1,
    };

    wrapper.vm.handleRowClick(testProject);
    await flushPromises();

    expect(Number(router.currentRoute.value.query.projectId)).toEqual(
      testProject.id,
    );
  });
});
