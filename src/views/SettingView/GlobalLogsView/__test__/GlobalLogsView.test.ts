import { createTestingPinia } from '@pinia/testing';
import { setActivePinia } from 'pinia';
import { describe } from 'vitest';
import { mount } from '@vue/test-utils';
import GlobalLogsView from '../GlobalLogsView.vue';
import { logsStoreSymbol } from '@/store/injectionSymbols';

describe('GlobalLogsView.vue', () => {
  setActivePinia(createTestingPinia({ stubActions: false }));
  it('fetches logs on mount', async () => {
    const fetchGlobalLogsMock = vi.fn();
    const logsStoreMock = {
      fetchGlobalLogs: fetchGlobalLogsMock,
    };
    mount(GlobalLogsView, {
      global: {
        provide: {
          [logsStoreSymbol as symbol]: logsStoreMock,
        },
      },
    });
    expect(fetchGlobalLogsMock).toHaveBeenCalledTimes(1);
  });
  it('refetches logs when user inputs a search term', async () => {
    const fetchGlobalLogsMock = vi.fn();
    const logsStoreMock = {
      fetchGlobalLogs: fetchGlobalLogsMock,
    };

    const wrapper = mount(GlobalLogsView, {
      global: {
        provide: {
          [logsStoreSymbol as symbol]: logsStoreMock,
        },
      },
    });
    expect(fetchGlobalLogsMock).toHaveBeenCalled();
    const searchInput = wrapper.find('.input');
    await searchInput.setValue('test search');
    await searchInput.trigger('change');
    await new Promise((resolve) => setTimeout(resolve, 1000));
    expect(fetchGlobalLogsMock).toHaveBeenCalledWith('test search');
  });
});
