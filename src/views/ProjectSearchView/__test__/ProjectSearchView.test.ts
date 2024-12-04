import { VueWrapper, flushPromises, mount } from '@vue/test-utils';
import ProjectSearchView from '../ProjectSearchView.vue';
import { describe, expect, it } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { usePluginsStore, useProjectStore } from '@/store';
import { setActivePinia } from 'pinia';
import _ from 'lodash';
import {
  pluginStoreSymbol,
  projectsStoreSymbol,
} from '@/store/injectionSymbols';
import type { ComponentPublicInstance } from 'vue';
import router from '@/router';
import type { ProjectModel } from '@/models/Project';
import { projectRoutingSymbol } from '@/store/injectionSymbols';
import { useProjectRouting } from '@/utils/hooks';

interface ProjectSearchViewInstance {
  paneWidth: number;
  paneHeight: number;
  handleRowClick: (project: ProjectModel) => void;
}

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
        [projectRoutingSymbol as symbol]: useProjectRouting(router),
      },
      plugins: [router],
    },
    propsData: {
      paneWidth: pWidth,
      paneHeight: 800,
    },
  });
};

describe('ProjectSearchView.vue', () => {
  setActivePinia(createTestingPinia());

  it('renders correctly with 4 columns', () => {
    const wrapper = generateWrapper(800);
    expect(wrapper.findAll('.ant-table-column-sorters')).toHaveLength(4);
  });

  it('renders correctly with reset button', async () => {
    const wrapper = generateWrapper(800);
    expect(wrapper.find('[name="resetButton"]').exists()).toBe(true);
  });

  it('hides columns when the pane width is not large enough', async () => {
    createTestingPinia({});
    const wrapper = generateWrapper(300);

    _.delay(
      () => expect(wrapper.findAll('.ant-table-column-sorters').length).toBe(2),
      1000,
    );
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

    await flushPromises();

    expect(projectStore.fetchProject).toHaveBeenCalledWith(300);
    expect(pluginStore.fetchPlugins).toHaveBeenCalledWith(300);
  });
});
