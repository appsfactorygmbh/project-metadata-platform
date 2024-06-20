import { describe, it, expect, vi, beforeEach } from 'vitest';
import { shallowMount } from '@vue/test-utils';
import { defineComponent, computed, toRaw } from 'vue';
import { setActivePinia, createPinia } from 'pinia';
import { usePluginsStore } from '@/store';
import type { PluginModel } from '@/models/Plugin';

// Mock the pluginService module
vi.mock('@/services/PluginService', () => ({
  pluginService: {
    fetchPlugins: vi.fn(),
  },
}));

// Define a PluginView component for testing
const PluginView = defineComponent({
  setup() {
    const pluginStore = usePluginsStore();
    const plugins = computed(() => toRaw(pluginStore.getPlugins));
    return { plugins };
  },
  template: '<div></div>',
});

describe('PluginView', () => {
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
      },
    ];

    // Set the store plugins
    store.setPlugins(mockPlugins);

    // Mount the PluginView component
    const wrapper = shallowMount(PluginView);

    // Access the computed plugins
    const computedPlugins = wrapper.vm.plugins;

    // Check if the computed plugins match the store plugins
    expect(computedPlugins).toEqual(mockPlugins);
  });
});
