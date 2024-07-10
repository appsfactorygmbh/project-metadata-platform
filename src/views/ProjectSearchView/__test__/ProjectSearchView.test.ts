import { flushPromises, mount } from '@vue/test-utils';
import ProjectSearchView from '../ProjectSearchView.vue';
import { describe, it, expect } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { usePluginsStore, useProjectStore, useSearchStore } from '@/store';
import { createPinia, setActivePinia } from 'pinia';
import _ from 'lodash';
import {
  pluginStoreSymbol,
  projectsStoreSymbol,
} from '@/store/injectionSymbols';
import router from '@/router';

const testProject = [
  {
    id: 100,
    projectName: 'Heute Show',
    clientName: 'ZDF',
    businessUnit: 'BU Health',
    teamNumber: 42,
  },
];

const testProjectInfo = {
  id: 100,
  projectName: 'Heute Show',
  department: 'IT',
  clientName: 'ZDF',
  businessUnit: 'BU Health',
  teamNumber: 42,
};

describe('ProjectSearchView.vue', () => {
  setActivePinia(createPinia());

  const generateWrapper = (pWidth: number) => {
    return mount(ProjectSearchView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            project: {
              project: testProjectInfo,
            },
            project_search: {
              baseSet: testProject,
            },
          },
        }),
      ],
      global: {
        provide: {
          [projectsStoreSymbol as symbol]: useProjectStore(),
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

  const wrapper = generateWrapper(800);

  const searchStore = useSearchStore('project');

  const loadData = async () => {
    searchStore.setBaseSet(testProject);
    searchStore.setSearchQuery('');
    await flushPromises();
  };

  it('renders correctly with 4 columns', async () => {
    await loadData();
    console.log(wrapper.html());

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
});
