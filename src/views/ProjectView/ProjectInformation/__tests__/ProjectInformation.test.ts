import { describe, expect, it } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import { setActivePinia } from 'pinia';
import ProjectInformation from '../ProjectInformation.vue';
import { createTestingPinia } from '@pinia/testing';
import {
  projectEditStoreSymbol,
  projectStoreSymbol,
} from '@/store/injectionSymbols';
import { useProjectEditStore, useProjectStore } from '@/store';
import router from '@/router';
import type { DetailedProjectModel } from '@/models/Project';
import {
  DeleteOutlined,
  EditOutlined,
  InboxOutlined,
  UndoOutlined,
} from '@ant-design/icons-vue';

const testData: DetailedProjectModel = {
  id: 1,
  slug: 'test-project',
  isArchived: false,
  projectName: 'Heute Show',
  department: 'IT',
  clientName: 'ZDF',
  businessUnit: 'BU Health',
  teamNumber: 42,
  offerId: '3',
  company: 'Appsfactory',
  companyState: 'EXTERNAL',
  ismsLevel: 'NORMAL',
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
    expect(wrapper.findAll('.projectInfo')[4].text()).toBe('3');
    expect(wrapper.findAll('.projectInfo')[5].text()).toBe('Appsfactory');
    expect(wrapper.findAll('.projectInfo')[6].text()).toBe('EXTERNAL');
    expect(wrapper.findAll('.projectInfo')[7].text()).toBe('NORMAL');
  });

  it('opens the confirmation modal when DeleteOutlined button is clicked', async () => {
    const testData = {
      projectName: 'Heute Show',
      department: 'IT',
      clientName: 'ZDF',
      businessUnit: 'BU Health',
      teamNumber: 42,
      isArchived: true,
      offerId: '3',
      company: 'Appsfactory',
      companyState: 'EXTERNAL',
      ismsLevel: 'NORMAL',
    };

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
          [projectStoreSymbol as symbol]: useProjectStore(),
        },
      },
    });

    await flushPromises();

    // check if delete button exists
    const deleteButton = wrapper.findComponent(DeleteOutlined);
    expect(deleteButton.exists()).toBeTruthy();

    // click on delete button
    await deleteButton.trigger('click');
    await flushPromises();

    // check if modal opened
    const confirmModal = wrapper.findComponent({ name: 'ConfirmAction' });
    expect(confirmModal.exists()).toBeTruthy();
    expect(confirmModal.props('isOpen')).toBe(true);
  });

  it('does not render the edit and archive button but shows the reactivate and delete button when archived', async () => {
    const testData = {
      projectName: 'Deutsche Bahn',
      department: 'IT',
      clientName: 'DB',
      businessUnit: 'DB Rail',
      teamNumber: 45,
      isArchived: true,
      offerId: '3',
      company: 'Appsfactory',
      companyState: 'EXTERNAL',
      ismsLevel: 'NORMAL',
    };

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
          [projectStoreSymbol as symbol]: useProjectStore(),
        },
      },
    });

    await flushPromises();

    // Expectation: No Edit- or Archive Button, but Reactivate and Delete Button
    expect(wrapper.findComponent(EditOutlined).exists()).toBeFalsy();
    expect(wrapper.findComponent(InboxOutlined).exists()).toBeFalsy();
    expect(wrapper.findComponent(UndoOutlined).exists()).toBeTruthy();
    expect(wrapper.findComponent(DeleteOutlined).exists()).toBeTruthy();
  });
});
