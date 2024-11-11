import { describe, expect, it } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import { setActivePinia } from 'pinia';
import ProjectInformation from '../ProjectInformation.vue';
import { createTestingPinia } from '@pinia/testing';
import { projectEditStoreSymbol } from '@/store/injectionSymbols';
import { useProjectEditStore, useProjectStore } from '@/store';
import router from '@/router';
import type { DetailedProjectModel } from '@/models/Project';

const testData: DetailedProjectModel = {
  id: 1,
  isArchived: false,
  projectName: 'Heute Show',
  department: 'IT',
  clientName: 'ZDF',
  businessUnit: 'BU Health',
  teamNumber: 42,
};

describe('ProjectInformation.vue', () => {
  const testingPinia = createTestingPinia({
    stubActions: false,
    initialState: {
      project: {
        project: testData,
      },
    },
  });
  setActivePinia(testingPinia);
  const projectStore = useProjectStore(testingPinia);

  const generateWrapper = () =>
    mount(ProjectInformation, {
      global: {
        plugins: [router, testingPinia],
        provide: {
          [projectEditStoreSymbol as symbol]: useProjectEditStore(),
        },
        stubs: {
          PluginView: {
            template: '<span />',
          },
        },
      },
    });

  it('renders the project information correctly', async () => {
    const wrapper = generateWrapper();
    await flushPromises();

    expect(projectStore.project).toMatchObject(testData);
    expect(wrapper.find('.projectName').text()).toEqual('Heute Show');
    expect(wrapper.findAll('.projectInfo')[0].text()).toBe('BU Health');
    expect(wrapper.findAll('.projectInfo')[1].text()).toBe('42');
    expect(wrapper.findAll('.projectInfo')[2].text()).toBe('IT');
    expect(wrapper.findAll('.projectInfo')[3].text()).toBe('ZDF');
  });
});
