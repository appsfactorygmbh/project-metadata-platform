import { mount } from '@vue/test-utils';
import { describe, it, expect, vi } from 'vitest';
import SearchBar from '../SearchBar.vue';
import { useSearchStore } from '@/store';
import { createTestingPinia } from '@pinia/testing';
import router from '@/router';

describe('SearchBar.vue', () => {
  createTestingPinia();

  beforeEach(() => {
    // Reset mock before each test
    vi.clearAllMocks();
  });

  it('renders correctly', () => {
    const wrapper = mount(SearchBar, {
      global: {
        plugins: [router],
      },
      propsData: { searchStoreSymbol: Symbol('searchStoreSym') },
    });
    expect(wrapper.exists()).toBe(true);
  });

  it('binds input value correctly to v-model', async () => {
    const wrapper = mount(SearchBar, {
      global: {
        plugins: [router],
      },
    });
    const input = wrapper.find('input');
    await input.setValue('Test');

    expect(input.element.value).toBe('Test');
  });

  it('calls searchStore when the user provides input', async () => {
    const searchStore = useSearchStore('test');
    const symbol = Symbol('searchStoreSym');

    const wrapper = mount(SearchBar, {
      global: {
        plugins: [createTestingPinia(), router],
        provide: {
          [symbol as symbol]: searchStore,
        },
      },
      propsData: { searchStoreSymbol: symbol },
    });
    const input = wrapper.find('input');
    await input.setValue('Test');

    // wait for all asynchronous calls to complete
    await wrapper.vm.$nextTick();
    await wrapper.vm.$nextTick();

    expect(searchStore.setSearchQuery).toHaveBeenCalled();
  });

  it('add query to router when searching', async () => {
    await router.isReady();

    const wrapper = mount(SearchBar, {
      global: {
        plugins: [router],
      },
    });

    const input = wrapper.find('input');
    await input.setValue('Test');

    expect(router.currentRoute.value.query.searchQuery).toBe('Test');
  });
});
