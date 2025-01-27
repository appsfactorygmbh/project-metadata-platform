import { flushPromises, mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import { ProjectView } from '..';
import router from '@/router';
import { EditOutlined, SaveOutlined } from '@ant-design/icons-vue';
import {
  localLogStoreSymbol,
  projectEditStoreSymbol,
} from '@/store/injectionSymbols';
import { useLocalLogStore, useProjectEditStore } from '@/store';
import { createTestingPinia } from '@pinia/testing';
import type { PluginModel } from '@/models/Plugin';
import ProjectEditButtons from '@/components/ProjectEditButtons/ProjectEditButtons.vue';
import type {
  DetailedProjectModel,
  UpdateProjectModel,
} from '@/models/Project';

const testPlugins: PluginModel[] = [
  {
    id: 1,
    pluginName: 'Test Plugin',
    displayName: 'Test Plugin',
    url: 'https://test.com',
  },
  {
    id: 2,
    pluginName: 'Test Plugin 2',
    displayName: 'Test Plugin 2',
    url: 'https://test2.com',
  },
];

const testUnarchivedPlugins: PluginModel[] = [
  {
    id: 1,
    pluginName: 'Test Plugin',
    displayName: 'Test Plugin',
    url: 'https://test.com',
  },
];

const testProject: DetailedProjectModel = {
  id: 1,
  projectName: 'Test Project',
  clientName: 'Test Client',
  businessUnit: 'Test Business Unit',
  department: 'Test Department',
  teamNumber: 1,
  isArchived: false,
  slug: 'test-project',
  offerId: '1',
  company: 'AppsFactory',
  companyState: 'EXTERNAL',
  ismsLevel: 'HIGH',
};

const testUpdatedProject: UpdateProjectModel = {
  projectName: 'Test Project',
  clientName: 'Test Client',
  businessUnit: 'Test Business Unit',
  department: 'Test Department',
  teamNumber: 1,
  pluginList: testPlugins,
  isArchived: false,
  offerId: '1',
  company: 'AppsFactory',
  companyState: 'EXTERNAL',
  ismsLevel: 'HIGH',
};

describe('ProjectView.vue', () => {
  it('doesnt delete archived plugins when editing', async () => {
    const logSpy = vi.spyOn(console, 'log');

    const wrapper = mount(ProjectView, {
      global: {
        provide: {
          [localLogStoreSymbol as symbol]: useLocalLogStore(),
          [projectEditStoreSymbol as symbol]: useProjectEditStore(),
        },
        plugins: [
          router,
          createTestingPinia({
            stubActions: false,
            initialState: {
              project: { project: testProject },
              plugin: {
                plugins: testPlugins,
                unarchivedPlugins: testUnarchivedPlugins,
              },
            },
          }),
        ],
      },
    });
    await flushPromises();

    const editButton = wrapper.findComponent(EditOutlined);
    editButton.trigger('click');
    await flushPromises();

    const saveButton = wrapper
      .findComponent(ProjectEditButtons)
      .findComponent(SaveOutlined);
    saveButton.trigger('click');
    await flushPromises();

    expect(logSpy).toHaveBeenNthCalledWith(2, testUpdatedProject);
  });
});
