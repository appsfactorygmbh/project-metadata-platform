import { describe, it, expect, vi, beforeEach } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';
import { usePluginsStore } from '@/store/Plugin/PluginStore';
import { pluginService } from '@/services/Plugin/PluginService';
import type { PluginType } from '@/models/PluginType';

// Mock the pluginService module
vi.mock('@/services/Plugin/PluginService', () => ({
  pluginService: {
    fetchPlugins: vi.fn(),
  },
}));

describe('usePluginsStore', () => {
  beforeEach(() => {
    // Set up a new Pinia store instance before each test
    setActivePinia(createPinia());
  });

  it('should fetch plugins and update the store', async () => {
    const store = usePluginsStore();
    const mockPlugins: PluginType[] = [
      {
        pluginName: 'testPlugin',
        displayName: 'Test Plugin',
        url: 'http://example.com/',
      },
    ];

    // Mock the fetchPlugins method
    (pluginService.fetchPlugins as ReturnType<typeof vi.fn>).mockResolvedValue(
      mockPlugins,
    );

    // Before the call: isLoading should be false
    expect(store.isLoading).toBe(false);

    // Call the fetchPlugins action
    const fetchPromise = store.fetchPlugins('testProjectID');

    // During the API call: isLoading should be true
    expect(store.isLoading).toBe(true);

    await fetchPromise;

    // Check if the store state is updated correctly
    expect(store.plugins).toEqual(mockPlugins);
    expect(store.isLoading).toBe(false);
    expect(pluginService.fetchPlugins).toHaveBeenCalledWith('testProjectID');
  });
});
