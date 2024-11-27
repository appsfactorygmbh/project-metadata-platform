import { beforeEach, describe, expect, it } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import { setActivePinia } from 'pinia';
import ProjectInformation from '../ProjectInformation.vue';
import { createTestingPinia } from '@pinia/testing';
import {
  projectEditStoreSymbol,
  projectsStoreSymbol,
} from '@/store/injectionSymbols';
import { useProjectEditStore, useProjectStore } from '@/store';
import router from '@/router';
import type { DetailedProjectModel } from '@/models/Project';
import {
  DeleteOutlined,
  EditOutlined,
  UndoOutlined,
} from '@ant-design/icons-vue';

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
  });

  it('opens the confirmation modal when DeleteOutlined button is clicked', async () => {
    const testData = {
      projectName: 'Heute Show',
      department: 'IT',
      clientName: 'ZDF',
      businessUnit: 'BU Health',
      teamNumber: 42,
      isArchived: false,
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
          [projectsStoreSymbol as symbol]: useProjectStore(),
        },
      },
    });

    await flushPromises();

    // Confirm Modal should be closed
    expect(
      wrapper.findComponent({ name: 'ConfirmAction' }).props('isOpen'),
    ).toBe(false);

    // find and clicks delete button
    await wrapper.find('.button .anticon-delete').trigger('click');
    await flushPromises();

    // Expectation: Confirm Modal is open
    expect(
      wrapper.findComponent({ name: 'ConfirmAction' }).props('isOpen'),
    ).toBe(true);
  });

  it('does not render the edit button but shows the reactivate button when archived', async () => {
    const testData = {
      projectName: 'Deutsche Bahn',
      department: 'IT',
      clientName: 'DB',
      businessUnit: 'DB Rail',
      teamNumber: 45,
      isArchived: true,
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
    await flushPromises();

    expect(wrapper.find('.projectName').text()).toBe('Heute Show');
    expect(wrapper.findAll('.projectInfo')[0].text()).toBe('BU Health');
    expect(wrapper.findAll('.projectInfo')[1].text()).toBe('42');
    expect(wrapper.findAll('.projectInfo')[2].text()).toBe('IT');
    expect(wrapper.findAll('.projectInfo')[3].text()).toBe('ZDF');
  });

  it('opens the confirmation modal when DeleteOutlined button is clicked', async () => {
    const testData = {
      projectName: 'Heute Show',
      department: 'IT',
      clientName: 'ZDF',
      businessUnit: 'BU Health',
      teamNumber: 42,
      isArchived: false,
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
          [projectsStoreSymbol as symbol]: useProjectStore(),
        },
      },
    });

    await flushPromises();

    // Confirm Modal should be closed
    expect(
      wrapper.findComponent({ name: 'ConfirmAction' }).props('isOpen'),
    ).toBe(false);

    // find and clicks delete button
    await wrapper.find('.button .anticon-delete').trigger('click');
    await flushPromises();

    // Expectation: Confirm Modal is open
    expect(
      wrapper.findComponent({ name: 'ConfirmAction' }).props('isOpen'),
    ).toBe(true);
  });

  it('does not render the edit button but shows the reactivate button when archived', async () => {
    const testData = {
      projectName: 'Deutsche Bahn',
      department: 'IT',
      clientName: 'DB',
      businessUnit: 'DB Rail',
      teamNumber: 45,
      isArchived: true,
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
          [projectsStoreSymbol as symbol]: useProjectStore(),
        },
      },
    });

    await flushPromises();

    // Expectation: No Edit- or Archive Button, but Reactivate Button
    expect(wrapper.findComponent(EditOutlined).exists()).toBeFalsy();
    expect(wrapper.findComponent(UndoOutlined).exists()).toBeTruthy();
    expect(wrapper.findComponent(DeleteOutlined).exists()).toBeFalsy();
  });
});
