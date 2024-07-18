import { describe, it, expect } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import { createPinia, setActivePinia } from 'pinia';
import ProjectInformation from '../ProjectInformation.vue';
import { createTestingPinia } from '@pinia/testing';
import {
  projectsStoreSymbol,
  projectEditStoreSymbol,
} from '@/store/injectionSymbols';
import { useProjectStore, useProjectEditStore } from '@/store';
import router from '@/router';

const testData = {
  projectName: 'Heute Show',
  department: 'IT',
  clientName: 'ZDF',
  businessUnit: 'BU Health',
  teamNumber: 42,
};

describe('ProjectInformationView.vue', () => {
  setActivePinia(createPinia());

  it('renders the project information correctly', async () => {
    const wrapper = mount(ProjectInformation, {
      plugins: [
        createTestingPinia({
          stubActions: true,
          initialState: {
            project: {
              project: testData,
            },
          },
        }),
      ],
      global: {
        stubs: {
          PluginView: {
            template: '<span />',
          },
        },
        plugins: [router],
        provide: {
          [projectEditStoreSymbol as symbol]: useProjectEditStore(),
          [projectsStoreSymbol as symbol]: useProjectStore(),
        },
      },
    });
    await flushPromises();

    expect(wrapper.find('.projectName').text()).toBe('Heute Show');
    expect(wrapper.findAll('.projectInfo')[0].text()).toBe('BU Health');
    expect(wrapper.findAll('.projectInfo')[1].text()).toBe('42');
    expect(wrapper.findAll('.projectInfo')[2].text()).toBe('IT');
    expect(wrapper.findAll('.projectInfo')[3].text()).toBe('ZDF');
  });
});
