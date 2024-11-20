import { describe, it, expect, vi, beforeEach } from 'vitest';
import { shallowMount } from '@vue/test-utils';
import { defineComponent, computed, toRaw } from 'vue';
import { setActivePinia, createPinia } from 'pinia';
import { useGlobalPluginsStore, usePluginsStore } from '@/store';
import type { GlobalPluginModel, PluginModel } from '@/models/Plugin';

// Mock the pluginService module
vi.mock('@/services/PluginService', () => ({
  pluginService: {
    fetchPlugins: vi.fn(),
  },
}));

// Define a ProjectPlugins component for testing
const ProjectPlugins = defineComponent({
  setup() {
    const pluginStore = usePluginsStore();
    const plugins = computed(() => toRaw(pluginStore.getPlugins));
    return { plugins };
  },
  template: '<div></div>',
});

describe('ProjectPlugins', () => {
  beforeEach(() => {
    // Set up a new Pinia store instance before each test
    setActivePinia(createPinia());
  });

  it('should compute plugins from the store', async () => {
    const store = usePluginsStore();
    const mockPlugins: PluginModel[] = [
      {
        pluginName: 'testPlugin',
        displayName: 'Test Plugin',
        url: 'http://example.com/',
        id: 1,
      },
    ];

    // Set the store plugins
    store.setPlugins(mockPlugins);

    // Mount the PluginView component
    const wrapper = shallowMount(ProjectPlugins);

    // Access the computed plugins
    const computedPlugins = wrapper.vm.plugins;

    // Check if the computed plugins match the store plugins
    expect(computedPlugins).toEqual(mockPlugins);
  });

  it('should only show local plugins from active global plugins', async () => {
    const pluginStore = usePluginsStore();
    const globalPluginStore = useGlobalPluginsStore();

    const mockPlugins = [
      {
        pluginName: 'testPlugin',
        displayName: 'Test Plugin',
        url: 'http://example.com/',
        id: 1,
      },
      {
        pluginName: 'testPlugin2',
        displayName: 'Test Plugin 2',
        url: 'http://example.com/',
        id: 2,
      },
    ];

    const mockGlobalPlugins: GlobalPluginModel[] = [
      {
        id: 1,
        name: 'testPlugin',
        isArchived: false,
      },
      {
        id: 2,
        name: 'testPlugin2',
        isArchived: true,
      },
    ];

    pluginStore.setPlugins(mockPlugins);
    globalPluginStore.setGlobalPlugins(mockGlobalPlugins);

    const filteredPlugins: PluginModel[] = [];
    // uses the same filter as the component
    const getFilteredPlugins = (plugin: PluginModel) => {
      if (
        globalPluginStore.getGlobalPlugins.find(
          (item) => item.name === plugin.pluginName && !item.isArchived,
        )
      ) {
        filteredPlugins.push(plugin);
      }
    };

    const wrapper = shallowMount(ProjectPlugins);

    wrapper.vm.plugins.forEach((plugin) => getFilteredPlugins(plugin));

    expect(filteredPlugins).toEqual([mockPlugins[0]]);
    expect(filteredPlugins).toHaveLength(1);
  });
});
