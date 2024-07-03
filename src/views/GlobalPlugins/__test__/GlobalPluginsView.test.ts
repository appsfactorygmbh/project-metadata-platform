import { createPinia, setActivePinia } from 'pinia';
import GlobalPluginsView from '../GlobalPluginsView.vue';
import { describe, it, expect } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import { createTestingPinia } from '@pinia/testing';
import { pluginStoreSymbol } from '@/store/injectionSymbols.ts';
import { usePluginsStore } from '@/store';

const testData = [
  {
    id: 0,
    name: 'Plugin 1',
    archieved: true,
  },
  {
    id: 1,
    name: 'Plugin 2',
    archieved: false,
  },
];

const testDataDelete = [
  {
    id: 0,
    name: 'Plugin 1',
    archieved: true,
  },
  {
    id: 1,
    name: 'Plugin 2',
    archieved: true,
  },
];

describe('GlobalPluginsView.vue', () => {
  setActivePinia(createPinia());

  const generateWrapper = () => {
    return mount(GlobalPluginsView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            plugin: {
              globalPlugins: testData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [pluginStoreSymbol as symbol]: usePluginsStore(),
        },
      },
    });
  };

  it('renders the fetched Plugins, but not the archieved ones', async () => {
    const wrapper = generateWrapper();
    await flushPromises();
    expect(wrapper.findAll('.ant-list-item')).toHaveLength(1);
    expect(wrapper.find('.ant-list-item').text()).toBe('Plugin 2');
    expect(wrapper.findAll('.ant-btn')).toHaveLength(2);
  });

  it('sends a delete request when clicking the delete button', async () => {
    const wrapper = generateWrapper();
    const pluginStore = usePluginsStore();
    const spy = vi.spyOn(pluginStore, 'deleteGlobalPlugin');
    spy.mockImplementation(async () =>
      pluginStore.setGlobalPlugins(testDataDelete),
    );

    await flushPromises();
    expect(wrapper.findAll('.ant-list-item')).toHaveLength(1);
    expect(spy).toHaveBeenCalledTimes(0);

    await wrapper.find('.anticon-delete').trigger('click');
    expect(wrapper.findAll('.ant-list-item')).toHaveLength(0);
    expect(spy).toHaveBeenCalledOnce();
  });
});
