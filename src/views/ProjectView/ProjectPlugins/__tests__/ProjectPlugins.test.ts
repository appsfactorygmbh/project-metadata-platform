import { describe, it, expect, vi, beforeEach } from 'vitest';
import { shallowMount } from '@vue/test-utils';
import { defineComponent, computed, toRaw } from 'vue';
import { setActivePinia, createPinia } from 'pinia';
import type { PluginModel } from '@/models/Plugin';
import { usePluginStore } from '@/store';

vi.mock('@/api/generated', async () => {
  const originalModule = await import('@/api/generated');
  return {
    ...originalModule,
    PluginsApi: vi.fn().mockImplementation(() => ({
      pluginsGet: vi.fn(),
    })),
  };
});

// Define a ProjectPlugins component for testing
const ProjectPlugins = defineComponent({
  setup() {
    const pluginStore = usePluginStore();
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
    const pluginStore = usePluginStore();
    const mockPlugins: PluginModel[] = [
      {
        pluginName: 'testPlugin',
        displayName: 'Test Plugin',
        url: 'http://example.com/',
        id: 1,
      },
    ];

    // Set the store plugins
    pluginStore.setPlugins(mockPlugins);

    // Mount the PluginView component
    const wrapper = shallowMount(ProjectPlugins);

    // Access the computed plugins
    const computedPlugins = wrapper.vm.plugins;

    // Check if the computed plugins match the store plugins
    expect(computedPlugins).toEqual(mockPlugins);
  });
});
