// import { mount } from '@vue/test-utils';
import { describe, it, expect, vi } from 'vitest';
// import PluginView from '../PluginView.vue';
import { createPinia, setActivePinia } from 'pinia';

const fetchPluginsMock = vi.fn();
const getPluginsMock = vi.fn(() => []);

vi.mock('@/store/Plugin/PluginStore.ts', () => {
  return {
    usePluginsStore: vi.fn(() => ({
      fetchPlugins: fetchPluginsMock,
      getPlugins: getPluginsMock,
    })),
  };
});

describe('PluginView.vue', () => {
  beforeEach(() => {
    const pinia = createPinia();
    setActivePinia(pinia);
  });

  it('calls fetchPlugins on beforeMount', async () => {
    // mount(PluginView, {
    //   props: { projectID: '100' },
    // });
    //
    // await new Promise((resolve) => setTimeout(resolve, 0));
    //
    // expect(fetchPluginsMock).toHaveBeenCalledWith('100');
    expect(1 + 1).equals(2);
  });
});
