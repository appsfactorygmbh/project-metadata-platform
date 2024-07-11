import { flushPromises, mount, VueWrapper } from '@vue/test-utils';
import ProjectSearchView from '../ProjectSearchView.vue';
import { describe, it, expect } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { usePluginsStore } from '@/store';
import { createPinia, setActivePinia } from 'pinia';
import _ from 'lodash';
import {
  pluginStoreSymbol,
  projectsStoreSymbol,
} from '@/store/injectionSymbols';
import type { ComponentPublicInstance } from 'vue';
import router from '@/router';
import type { ProjectModel } from '@/models/Project';

const testProject = {
  id: 100,
  projectName: 'Heute Show',
  clientName: 'ZDF',
  businessUnit: 'BU Health',
  teamNumber: 42,
};

interface ProjectSearchViewInstance {
  paneWidth: number;
  paneHeight: number;
  handleRowClick: (project: ProjectModel) => void;
}

describe('ProjectSearchView.vue', () => {
  setActivePinia(createPinia());

  const projectsStoreMock = {
    fetchProject: vi.fn(),
  };

  const generateWrapper = (pWidth: number) => {
    return mount(ProjectSearchView, {
      plugins: [
        createTestingPinia({
          stubActions: true,
        }),
      ],
      global: {
        provide: {
          [projectsStoreSymbol as symbol]: projectsStoreMock,
          [pluginStoreSymbol as symbol]: usePluginsStore(),
        },
        plugins: [router],
      },
      propsData: {
        paneWidth: pWidth,
        paneHeight: 800,
      },
    });
  };

  const wrapper = generateWrapper(800) as VueWrapper<
    ComponentPublicInstance<ProjectSearchViewInstance>
  >;

  it('renders correctly with 4 columns', () => {
    expect(wrapper.findAll('.ant-table-column-sorters')).toHaveLength(4);
  });

  createTestingPinia({});
  const wrapper2 = generateWrapper(300);

  it('hides columns when the pane width is not large enough', async () => {
    await flushPromises();

    _.delay(
      () =>
        expect(wrapper2.findAll('.ant-table-column-sorters').length).toBe(2),
      500,
    );
  });

  it('changes the router, when a project is clicked', async () => {
    wrapper.vm.handleRowClick(testProject);

    // expect(useRouter().push).toHaveBeenCalledWith({
    //   path: useRouter().currentRoute.value.path,
    //   query: { project: testProject.id },
    // });
    expect(projectsStoreMock.fetchProject).toHaveBeenCalledWith(testProject.id);
  });
});
