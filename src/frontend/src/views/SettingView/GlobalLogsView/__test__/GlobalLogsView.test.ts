import { createTestingPinia } from '@pinia/testing';
import { setActivePinia } from 'pinia';
import { describe } from 'vitest';
import { mount } from '@vue/test-utils';
import GlobalLogsView from '../GlobalLogsView.vue';
import { logsStoreSymbol } from '@/store/injectionSymbols';

describe('GlobalLogsView.vue', () => {
  setActivePinia(createTestingPinia({ stubActions: false }));
  it('render correctly', async () => {
    const fetchMock = vi.fn();
    const logsStoreMock = {
      fetch: fetchMock,
    };
    const wrapper = mount(GlobalLogsView, {
      global: {
        provide: {
          [logsStoreSymbol as symbol]: logsStoreMock,
        },
      },
    });
    expect(wrapper.find('.time').exists());
    expect(wrapper.find('.cardContainer').exists());
  });
  it('fetches logs on mount', async () => {
    const fetchMock = vi.fn();
    const logsStoreMock = {
      fetch: fetchMock,
    };
    mount(GlobalLogsView, {
      global: {
        provide: {
          [logsStoreSymbol as symbol]: logsStoreMock,
        },
      },
    });
    expect(fetchMock).toHaveBeenCalledTimes(1);
  });
  it('refetches logs when user inputs a search term', async () => {
    const fetchMock = vi.fn();
    const logsStoreMock = {
      fetch: fetchMock,
    };

    const wrapper = mount(GlobalLogsView, {
      global: {
        provide: {
          [logsStoreSymbol as symbol]: logsStoreMock,
        },
      },
    });
    expect(fetchMock).toHaveBeenCalled();
    const searchInput = wrapper.find('.input');
    await searchInput.setValue('test search');
    await searchInput.trigger('change');
    await new Promise((resolve) => setTimeout(resolve, 1000));
    expect(fetchMock).toHaveBeenCalledWith('test search');
  });
});
