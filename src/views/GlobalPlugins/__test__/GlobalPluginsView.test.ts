import { createPinia, setActivePinia } from 'pinia';
import GlobalPluginsView from '../GlobalPluginsView.vue';
import { describe, expect, it } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import { createTestingPinia } from '@pinia/testing';
import { globalPluginStoreSymbol } from '@/store/injectionSymbols.ts';
import { useGlobalPluginsStore } from '@/store';
import { Button } from 'ant-design-vue';

const testData = [
  {
    id: 0,
    name: 'Plugin 1',
    isArchived: true,
  },
  {
    id: 1,
    name: 'Plugin 2',
    isArchived: false,
  },
];

const testDataDelete = [
  {
    id: 0,
    name: 'Plugin 1',
    isArchived: true,
  },
  {
    id: 1,
    name: 'Plugin 2',
    isArchived: true,
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
            globalPlugin: {
              globalPlugins: testData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [globalPluginStoreSymbol as symbol]: useGlobalPluginsStore(),
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
    const globalPluginStore = useGlobalPluginsStore();
    const spy = vi.spyOn(globalPluginStore, 'archiveGlobalPlugin');
    spy.mockImplementation(async () =>
      globalPluginStore.setGlobalPlugins(testDataDelete),
    );

    await flushPromises();
    expect(wrapper.findAll('.ant-list-item')).toHaveLength(1);
    expect(spy).toHaveBeenCalledTimes(0);

    await wrapper.find('.anticon-delete').trigger('click');
    const confirmButton = wrapper.findAllComponents(Button)[3];
    await confirmButton.trigger('click');

    expect(wrapper.findAll('.ant-list-item')).toHaveLength(0);
    expect(spy).toHaveBeenCalledOnce();
  });
});
