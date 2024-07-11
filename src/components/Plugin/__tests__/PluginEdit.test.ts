import { describe, it, expect, beforeEach } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';
import { usePluginsStore } from '@/store/PluginStore';

describe('PluginStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia());
  });
  it('updates plugin URL correctly', () => {
    const store = usePluginsStore();
    store.setPlugins([
      {
        id: 1,
        pluginName: 'Plugin 1',
        displayName: 'Display 1',
        url: 'http://old.url',
      },
      {
        id: 2,
        pluginName: 'Plugin 2',
        displayName: 'Display 2',
        url: 'http://example.com',
      },
    ]);
    store.updatePluginURL(1, 'http://old.url', 'http://new.url');

    const cachedPlugin = store.cachePlugins.find((plugin) => plugin.id === 1);
    expect(cachedPlugin?.url).toBe('http://new.url');
  });

  it('updates display correctly', () => {
    const store = usePluginsStore();
    store.setPlugins([
      {
        id: 1,
        pluginName: 'Plugin 1',
        displayName: 'Display 1',
        url: 'http://old.url',
      },
      {
        id: 2,
        pluginName: 'Plugin 2',
        displayName: 'Display 2',
        url: 'http://example.com',
      },
    ]);
    store.updateDisplayName(1, 'http://old.url', 'New Display Name');

    console.log(store.cachePlugins);

    const cachedPlugin = store.cachePlugins.find((plugin) => plugin.id === 2);
    expect(cachedPlugin?.displayName).toBe('New Display Name');
  });
});
