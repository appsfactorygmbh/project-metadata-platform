import { setActivePinia } from 'pinia';
import GlobalPluginsView from '../GlobalPluginsView.vue';
import { describe, expect, it } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import { createTestingPinia } from '@pinia/testing';
import { useGlobalPluginsStore } from '@/store';
import { Button } from 'ant-design-vue';
import type { GlobalPluginModel } from '@/models/Plugin';

const testData: GlobalPluginModel[] = [
  {
    id: 0,
    name: 'Plugin 1',
    isArchived: true,
    keys: [],
  },
  {
    id: 1,
    name: 'Plugin 2',
    isArchived: false,
    keys: [],
  },
];

const testDataArchive: GlobalPluginModel[] = [
  {
    id: 0,
    name: 'Plugin 1',
    isArchived: true,
    keys: [],
  },
  {
    id: 1,
    name: 'Plugin 2',
    isArchived: false,
    keys: [],
  },
];

const testDataReactivate: GlobalPluginModel[] = [
  {
    id: 0,
    name: 'Plugin 1',
    isArchived: true,
    keys: [],
  },
  {
    id: 1,
    name: 'Plugin 2',
    isArchived: true,
    keys: [],
  },
];

describe('GlobalPluginsView.vue', () => {
  const testingPinia = createTestingPinia({
    stubActions: false,
    initialState: {
      globalPlugin: {
        globalPlugins: testData,
        fetchAll: vi.fn(), // prevent call to api
      },
    },
  });
  setActivePinia(testingPinia);

  const globalPluginStore = useGlobalPluginsStore(testingPinia);

  const generateWrapper = () => {
    return mount(GlobalPluginsView, { global: { plugins: [testingPinia] } });
  };

  it('renders the fetched Plugins, but not the archieved ones', async () => {
    const wrapper = generateWrapper();
    await flushPromises();
    expect(globalPluginStore.getGlobalPlugins).toMatchObject(testData);
    expect(wrapper.findAll('.ant-list-item')).toHaveLength(1);
    expect(wrapper.find('.ant-list-item').text()).toBe('Plugin 2');
    expect(wrapper.findAll('.ant-btn')).toHaveLength(3);
  });

  it('switches to archived plugins when clicking the button', async () => {
    const wrapper = generateWrapper();
    const archiveButton = wrapper.findComponent(Button);
    await archiveButton.trigger('click');
    await flushPromises();

    expect(wrapper.findAll('.ant-list-item')).toHaveLength(1);
    expect(wrapper.find('.ant-list-item').text()).toBe('Plugin 1');
  });

  it('calls the store when clicking the archive button', async () => {
    const wrapper = generateWrapper();
    const globalPluginStore = useGlobalPluginsStore();
    const spy = vi.spyOn(globalPluginStore, 'archiveGlobalPlugin');
    spy.mockImplementation(async () =>
      globalPluginStore.setGlobalPlugins(testDataArchive),
    );

    await flushPromises();
    expect(wrapper.findAll('.ant-list-item')).toHaveLength(1);
    expect(spy).toHaveBeenCalledTimes(0);

    await wrapper.findAll('.anticon-inbox')[1].trigger('click');
    //confirms the action
    const confirmButton = wrapper.findAllComponents(Button)[4];
    await confirmButton.trigger('click');

    expect(wrapper.findAll('.ant-list-item')).toHaveLength(0);
    expect(spy).toHaveBeenCalledOnce();
  });

  it('calls the store when clicking the reactivate button', async () => {
    const wrapper = generateWrapper();
    const globalPluginStore = useGlobalPluginsStore();
    const spy = vi.spyOn(globalPluginStore, 'reactivateGlobalPlugin');
    spy.mockImplementation(async () =>
      globalPluginStore.setGlobalPlugins(testDataReactivate),
    );

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
    const globalPluginStore = useGlobalPluginsStore();
    const spy = vi.spyOn(globalPluginStore, 'deleteGlobalPlugin');
    spy.mockImplementation(async () =>
      globalPluginStore.setGlobalPlugins(testData),
    );

    await flushPromises();
    expect(spy).toHaveBeenCalledTimes(0);

    //toggles archived plugins and deletes the first one
    await wrapper.find('.anticon-inbox').trigger('click');
    await wrapper.find('.anticon-delete').trigger('click');
    //confirms the action
    const confirmButton = wrapper.findAllComponents(Button)[4];
    await confirmButton.trigger('click');

    expect(spy).toHaveBeenCalledOnce();
  });
});
