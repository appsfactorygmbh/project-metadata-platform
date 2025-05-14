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
  slug: 'test_project',
  isArchived: false,
  projectName: 'Heute Show',
  clientName: 'ZDF',
  team: {
    id: 1,
    businessUnit: 'BU Health',
    teamName: '42',
  },
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
      global: {
        plugins: [router, testingPinia],
        provide: {
          [projectEditStoreSymbol as symbol]: useProjectEditStore(),
          [projectStoreSymbol as symbol]: useProjectStore(),
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
    expect(wrapper.findAll('.infoCard')[0].text()).toBe(
      'Project\xa0Slug:test_project',
    );
    expect(wrapper.findAll('.infoCard')[1].text()).toBe(
      'Business\xa0Unit:BU Health',
    );
    expect(wrapper.findAll('.infoCard')[2].text()).toBe('Team\xa0Number:42');
    expect(wrapper.findAll('.infoCard')[3].text()).toBe('Department:IT');
    expect(wrapper.findAll('.infoCard')[4].text()).toBe('Client\xa0Name:ZDF');
    expect(wrapper.findAll('.infoCard')[5].text()).toBe('Offer\xa0ID:3');
    expect(wrapper.findAll('.infoCard')[6].text()).toBe('Company:Appsfactory');
    expect(wrapper.findAll('.infoCard')[7].text()).toBe(
      'Company\xa0State:External',
    );
    expect(wrapper.findAll('.infoCard')[8].text()).toBe('ISMS\xa0Level:Normal');
  });

  it('opens the confirmation modal when DeleteOutlined button is clicked', async () => {
    projectStore.project!.isArchived = true;

    const wrapper = generateWrapper();
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
    const wrapper = generateWrapper();
    await flushPromises();

    // Expectation: No Edit- or Archive Button, but Reactivate and Delete Button
    expect(wrapper.findComponent(EditOutlined).exists()).toBeFalsy();
    expect(wrapper.findComponent(InboxOutlined).exists()).toBeFalsy();
    expect(wrapper.findComponent(UndoOutlined).exists()).toBeTruthy();
    expect(wrapper.findComponent(DeleteOutlined).exists()).toBeTruthy();
  });
});
