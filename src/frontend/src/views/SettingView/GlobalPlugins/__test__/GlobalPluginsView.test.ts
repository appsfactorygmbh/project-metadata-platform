import { getActivePinia, setActivePinia } from 'pinia';
import GlobalPluginsView from '../GlobalPluginsView.vue';
import { describe, expect, it } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import { createTestingPinia } from '@pinia/testing';
import {
  type GlobalPluginStore,
  useAuthStore,
  useGlobalPluginStore,
  usePluginStore,
} from '@/store';
import { Button } from 'ant-design-vue';
import type { GlobalPluginModel } from '@/models/GlobalPlugin';
import {
  authStoreSymbol,
  globalPluginStoreSymbol,
  pluginStoreSymbol,
} from '@/store/injectionSymbols';
import router from '@/router';
import { useStore } from 'pinia-generic';
import { ResourceActions } from '@/models/utils/ResourceActions.ts';
import ConfirmationDialog from '@/components/Modal/ConfirmAction.vue';

const testData: GlobalPluginModel[] = [
  {
    id: 2,
    pluginName: 'Plugin 1',
    isArchived: true,
    keys: [],
    baseUrl: 'plugin1.de',
    permissions: [ResourceActions.Edit, ResourceActions.Delete],
  },
  {
    id: 1,
    pluginName: 'Plugin 2',
    isArchived: false,
    keys: [],
    baseUrl: 'plugin2.de',
    permissions: [ResourceActions.Edit, ResourceActions.Delete],
  },
];

const testDataArchive: GlobalPluginModel[] = [
  {
    id: 0,
    pluginName: 'Plugin 1',
    isArchived: true,
    keys: [],
    baseUrl: 'plugin1.de',
    permissions: [ResourceActions.Edit, ResourceActions.Delete],
  },
  {
    id: 1,
    pluginName: 'Plugin 2',
    isArchived: true,
    keys: [],
    baseUrl: 'plugin2.de',
    permissions: [ResourceActions.Edit, ResourceActions.Delete],
  },
];

const testDataReactivate: GlobalPluginModel[] = [
  {
    id: 0,
    pluginName: 'Plugin 1',
    isArchived: true,
    keys: [],
    baseUrl: 'plugin1.de',
    permissions: [ResourceActions.Edit, ResourceActions.Delete],
  },
  {
    id: 1,
    pluginName: 'Plugin 2',
    isArchived: true,
    keys: [],
    baseUrl: 'plugin2.de',
    permissions: [ResourceActions.Edit, ResourceActions.Delete],
  },
];

const piniaOptions: Parameters<typeof createTestingPinia>[0] = {
  stubActions: false,
  initialState: {
    globalPlugin: {
      // globalPlugins: testData,
      // getGlobalPlugins: vi.fn(() => testData),
      fetch: vi.fn(), // prevent call to api
      fetchAll: vi.fn(), // prevent call to api
      // setGlobalPlugins: vi.fn(),
    },
  },
};
const testingPinia = createTestingPinia(piniaOptions);

describe('GlobalPluginsView.vue', () => {
  setActivePinia(testingPinia);

  beforeAll(() => {
    vi.mock('@/store/GlobalPluginStore', async (importOriginal) => {
      return {
        ...(await importOriginal<typeof import('@/store/GlobalPluginStore')>()),
        useGlobalPluginStore: (pinia = testingPinia) => {
          return useStore<GlobalPluginStore>('globalPlugin', {
            state: {
              globalPlugins: testData,
            },
            getters: {
              getGlobalPlugins() {
                return this.globalPlugins;
              },
              getPermissions() {
                return [ResourceActions.Create];
              },
            },
            actions: {
              fetch: vi.fn(),
              fetchAll: vi.fn(),
              refreshAuth: vi.fn(),
              setGlobalPlugins(plugins) {
                this.globalPlugins = plugins;
              },
              async archive(id) {
                this.globalPlugins = testDataArchive;
              },
              async unarchive(id) {
                this.globalPlugins = testDataReactivate;
              },
              delete: vi.fn().mockImplementation(() => {}),
            },
          })(getActivePinia());
        },
      };
    });
  });

  const globalPluginStore = useGlobalPluginStore(testingPinia);

  const generateWrapper = () => {
    return mount(GlobalPluginsView, {
      global: {
        plugins: [createTestingPinia(piniaOptions), router],
        provide: {
          [globalPluginStoreSymbol as symbol]: globalPluginStore,
          [pluginStoreSymbol as symbol]: usePluginStore(testingPinia),
          [authStoreSymbol as symbol]: useAuthStore(testingPinia),
        },
      },
    });
  };

  it('renders the fetched Plugins, but not the archived ones', async () => {
    const wrapper = generateWrapper();
    await flushPromises();
    expect(globalPluginStore.getGlobalPlugins).toMatchObject(testData);
    expect(wrapper.findAll('.ant-list-item')).toHaveLength(1);
    expect(wrapper.find('.ant-list-item').text()).toBe('Plugin 2plugin2.de');
  });

  it('switches to archived plugins when clicking the button', async () => {
    const wrapper = generateWrapper();
    const archiveButton = wrapper.findComponent(Button);
    await archiveButton.trigger('click');
    await flushPromises();

    expect(wrapper.findAll('.ant-list-item')).toHaveLength(1);
    expect(wrapper.find('.ant-list-item').text()).toBe('Plugin 1plugin1.de');
  });

  it('calls the store when clicking the reactivate button', async () => {
    const wrapper = generateWrapper();
    const globalPluginStore = useGlobalPluginStore();
    const spy = vi.spyOn(globalPluginStore, 'unarchive');

    await flushPromises();
    expect(wrapper.findAll('.ant-list-item')).toHaveLength(1);
    expect(spy).toHaveBeenCalledTimes(0);

    //toggles archived plugins and reactivates the first one
    await wrapper.find('.anticon-inbox').trigger('click');
    await wrapper.find('.anticon-undo').trigger('click');

    expect(wrapper.findAll('.ant-list-item')).toHaveLength(2);
    expect(spy).toHaveBeenCalledOnce();
  });

  it('calls the store when clicking the delete button', async () => {
    const wrapper = generateWrapper();
    const globalPluginStore = useGlobalPluginStore();
    const spy = vi.spyOn(globalPluginStore, 'delete');

    await flushPromises();
    expect(spy).toHaveBeenCalledTimes(0);

    //toggles archived plugins and deletes the first one
    await wrapper.find('.anticon-inbox').trigger('click');
    await flushPromises();
    await wrapper.find('.anticon-delete').trigger('click');
    await flushPromises();
    //confirms the action
    const dialog = wrapper.findComponent(ConfirmationDialog);
    await dialog.vm.$emit('confirm');
    await flushPromises();

    expect(spy).toHaveBeenCalledOnce();
  });
});
