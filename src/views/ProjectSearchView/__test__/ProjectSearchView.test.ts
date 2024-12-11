import { VueWrapper, flushPromises, mount } from '@vue/test-utils';
import ProjectSearchView from '../ProjectSearchView.vue';
import { describe, expect, it } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { useLocalLogStore, usePluginStore, useProjectStore } from '@/store';
import { setActivePinia } from 'pinia';
import {
  localLogStoreSymbol,
  projectRoutingSymbol,
} from '@/store/injectionSymbols';
import type { ComponentPublicInstance } from 'vue';
import router from '@/router';
import type { ProjectModel } from '@/models/Project';
import { useProjectRouting } from '@/utils/hooks';

interface ProjectSearchViewInstance {
  paneWidth: number;
  paneHeight: number;
  handleRowClick: (project: ProjectModel) => void;
}

describe('ProjectSearchView.vue', () => {
  const generateWrapper = (pWidth: number) => {
    return mount(ProjectSearchView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        provide: {
          [projectRoutingSymbol as symbol]: useProjectRouting(router),
          [localLogStoreSymbol as symbol]: useLocalLogStore(),
        },
        plugins: [router],
      },
      propsData: {
        paneWidth: pWidth,
        paneHeight: 800,
      },
    });
  };

  setActivePinia(createTestingPinia());

  it('renders correctly with 4 columns', () => {
    const wrapper = generateWrapper(800);
    expect(wrapper.findAll('.ant-table-column-sorters')).toHaveLength(4);
  });

  it('renders correctly with reset button', async () => {
    const wrapper = generateWrapper(800);
    expect(wrapper.find('[name="resetButton"]').exists()).toBe(true);
  });

  // TODO: This test is flaky and fails on CI because of the time based transition
  it.skip('hides columns when the pane width is not large enough', async () => {
    createTestingPinia({});
    const wrapper = generateWrapper(300);
    await flushPromises();
    await new Promise((resolve) => setTimeout(resolve, 2000));
    expect(wrapper.findAll('.ant-table-column-sorters')).toHaveLength(2);
  });

  it('adds a query when clicking on a project', async () => {
    await router.isReady();

    const wrapper = generateWrapper(700) as VueWrapper<
      ComponentPublicInstance<ProjectSearchViewInstance>
    >;
    const testProject: ProjectModel = {
      id: 200,
      projectName: 'test',
      clientName: 'test',
      businessUnit: 'test',
      teamNumber: 1,
      isArchived: false,
      slug: 'test-project',
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

    const pluginStore = usePluginStore();
    const projectStore = useProjectStore();
    generateWrapper(800);
    await flushPromises();

    await flushPromises();

    expect(projectStore.fetch).toHaveBeenCalledWith(300);
    expect(pluginStore.fetch).toHaveBeenCalledWith(300);
  });
});
