import { mount, flushPromises } from '@vue/test-utils';
import { describe, it, expect, vi } from 'vitest';
import SearchBar from '../SearchBar.vue';
import { useSearchStore } from '@/store';
import { createPinia, setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import router from '@/router';

describe('SearchBar.vue', () => {
  setActivePinia(createPinia());
  const searchStoreSymbol = Symbol('searchStoreSym');
  const searchStore = useSearchStore('test');

  const wrapper = mount(SearchBar, {
    plugins: [
      createTestingPinia({
        stubActions: false,
      }),
    ],
    global: {
      plugins: [router],
      provide: {
        [searchStoreSymbol as symbol]: searchStore,
      },
    },
    propsData: {
      searchStoreSymbol: searchStoreSymbol,
    },
  });

  beforeEach(() => {
    // Reset mock before each test
    vi.clearAllMocks();
  });

  it('renders correctly', () => {
    expect(wrapper.exists()).toBe(true);
  });

  it('binds input value correctly to v-model', async () => {
    const input = wrapper.find('input');
    await input.setValue('Test');

    expect(input.element.value).toBe('Test');
  });

  it('calls searchStore when the user provides input', async () => {
    const input = wrapper.find('input');
    await input.setValue('Test');

    // wait for all asynchronous calls to complete
    await flushPromises();
    expect(searchStore.getSearchQuery).toEqual('Test');
  });

  it('reset the searchBar when using the searchStore reset ', async () => {
    const input = wrapper.find('input');

    await input.setValue('C');
    await flushPromises();
    expect(searchStore.getSearchQuery).toEqual('C');
    expect(input.element.value).toBe('C');

    searchStore.reset();
    await flushPromises();
    expect(searchStore.getSearchQuery).toEqual('');
    expect(input.element.value).toBe('');
  });

  it('add query to router when searching', async () => {
    await router.isReady();

    const wrapper = mount(SearchBar, {
      global: {
        plugins: [router],
      },
    });

    const input = wrapper.find('input');
    await input.setValue('Test1');
    await flushPromises();

    expect(router.currentRoute.value.query.searchQuery).toBe('Test1');
  });

  it.todo(
    'sets the default value and calls searchStore with query in URL',
    async () => {
      await router.push({
        path: '/',
        query: { searchQuery: 'Test2' },
      });
      await router.isReady();

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

      expect(input.element.value).toBe('Test2');
      expect(searchStore.setSearchQuery).toHaveBeenCalledWith('Test2');
    },
  );
});
