// YourComponent.test.js
import { mount } from '@vue/test-utils';
import PluginView from '../PluginView.vue';
import { usePluginsStore } from '../../../store/Plugin/PluginStore.ts';
import { vi } from 'vitest';
import { nextTick } from 'vue';

// Mock the usePluginsStore
vi.mock('../../store/Plugin/PluginStore.ts', () => {
  return {
    usePluginsStore: vi.fn(),
  };
});

describe('PluginView', () => {
  let fetchPluginsMock;
  let getPluginsMock;

  beforeEach(() => {
    fetchPluginsMock = vi.fn();
    getPluginsMock = vi.fn(() => [
      {
        pluginName: 'plugin1',
        displayName: 'Plugin 1',
        url: 'http://plugin1.com',
      },
      {
        pluginName: 'plugin2',
        displayName: 'Plugin 2',
        url: 'http://plugin2.com',
      },
    ]);

    // Mock the return value of usePluginsStore
    usePluginsStore.mockReturnValue({
      fetchPlugins: fetchPluginsMock,
      getPlugins: getPluginsMock,
    });
  });

  test('calls fetchPlugins on mount and renders plugins', async () => {
    const projectID = '12345';
    const wrapper = mount(PluginView, {
      props: { projectID },
    });

    // Ensure fetchPlugins is called
    expect(fetchPluginsMock).toHaveBeenCalledWith(projectID);

    // Wait for any pending promises
    await nextTick();

    // Ensure the plugins are rendered
    const pluginComponents = wrapper.findAllComponents({ name: 'PluginComponent' });
    expect(pluginComponents.length).toBe(2);

    // Check the props of the first plugin component
    expect(pluginComponents[0].props()).toMatchObject({
      pluginName: 'plugin1',
      displayName: 'Plugin 1',
      url: 'http://plugin1.com',
    });
  });
});
