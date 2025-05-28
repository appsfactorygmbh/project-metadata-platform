import { flushPromises, mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import { ProjectView } from '..';
import router from '@/router';
import { EditOutlined } from '@ant-design/icons-vue';
import {
  localLogStoreSymbol,
  projectEditStoreSymbol,
  projectStoreSymbol,
  teamStoreSymbol,
} from '@/store/injectionSymbols';
import {
  useLocalLogStore,
  useProjectEditStore,
  useProjectStore,
  useTeamStore,
} from '@/store';
import { createTestingPinia } from '@pinia/testing';
import type { PluginModel } from '@/models/Plugin';
import type { DetailedProjectModel } from '@/models/Project';

vi.mock('vue-auth3', () => ({
  useAuth: () => ({
    ready: vi.fn().mockResolvedValue(undefined),
    check: vi.fn().mockReturnValue(true),
  }),
}));

vi.mock('vue-auth3', () => ({
  useAuth: () => ({
    ready: vi.fn().mockResolvedValue(undefined),
    check: vi.fn().mockReturnValue(true),
  }),
}));

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
  team: {
    businessUnit: 'Test Business Unit',
    id: 1,
    teamName: '1',
  },
  isArchived: false,
  slug: 'test_project',
  offerId: '1',
  company: 'AppsFactory',
  companyState: 'EXTERNAL',
  ismsLevel: 'HIGH',
};

const pinia = createTestingPinia({
  stubActions: false,
  initialState: {
    project: { project: testProject },
    plugin: {
      plugins: testPlugins,
      unarchivedPlugins: testUnarchivedPlugins,
    },
    team: {
      teams: [
        {
          businessUnit: 'Test Business Unit',
          id: 1,
          teamName: '1',
        },
      ],
    },
  },
});

describe('ProjectView.vue', () => {
  const generateWrapper = () => {
    return mount(ProjectView, {
      global: {
        provide: {
          [localLogStoreSymbol as symbol]: useLocalLogStore(),
          [projectEditStoreSymbol as symbol]: useProjectEditStore(),
          [teamStoreSymbol as symbol]: useTeamStore(),
          [projectStoreSymbol as symbol]: useProjectStore(),
        },
        plugins: [router, pinia],
      },
    });
  };

  it('hides the project slug + team fields when editing', async () => {
    const wrapper = generateWrapper();
    await flushPromises();

    // check if fields are visible
    expect(wrapper.find('.label').text()).toBe('Project\xa0Slug:');
    expect(wrapper.find('.teamNameField').exists()).toBe(true);
    expect(wrapper.find('.buField').exists()).toBe(true);
    expect(wrapper.find('.ptlField').exists()).toBe(true);

    // click on edit button
    const editButton = wrapper.findComponent(EditOutlined);
    await editButton.trigger('click');
    await flushPromises();

    // check if fields are hidden
    expect(wrapper.find('.label').text()).not.toBe('Project\xa0Slug:');
    expect(wrapper.find('.teamNameField').exists()).toBe(false);
    expect(wrapper.find('.buField').exists()).toBe(false);
    expect(wrapper.find('.ptlField').exists()).toBe(false);
  });
});
