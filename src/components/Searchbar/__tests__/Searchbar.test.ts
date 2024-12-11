import { mount, flushPromises } from '@vue/test-utils';
import { describe, it, expect, vi } from 'vitest';
import SearchBar from '../SearchBar.vue';
import { useSearchStore } from '@/store';
import { createTestingPinia } from '@pinia/testing';
import router from '@/router';
import { createPinia, setActivePinia } from 'pinia';

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
    await wrapper.vm.$nextTick();
    await wrapper.vm.$nextTick();

    expect(searchStore.getSearchQuery).toBe('Test');
  });

  it('stores the input in the searchStorage', async () => {
    const input = wrapper.find('input');
    await input.setValue('Test1');

    const searchQuery = JSON.parse(
      sessionStorage.getItem('searchStorage')!,
    ).searchQuery;

    expect(searchQuery).toBe('Test1');
  });

  it('reset the searchBar when using the searchStore reset ', async () => {
    const input = wrapper.find('input');
    input.setValue('C');
    await flushPromises();

    expect(searchStore.getSearchQuery).toBe('C');
    expect(input.element.value).toBe('C');

    searchStore.reset();
    await flushPromises();
    expect(searchStore.getSearchQuery).toBe('');
    expect(input.element.value).toBe('');
  });

  it('add query to router when searching', async () => {
    await router.isReady();

    const wrapper = mount(SearchBar, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        provide: {
          [searchStoreSymbol as symbol]: searchStore,
        },
        plugins: [router],
      },
      propsData: {
        searchStoreSymbol: searchStoreSymbol,
      },
    });

    const input = wrapper.find('input');
    await input.setValue('Test1');
    await flushPromises();

    expect(router.currentRoute.value.query.searchQuery).toBe('Test1');
    wrapper.unmount();
  });

  it('sets the default value and calls searchStore with query in URL', async () => {
    await router.push({
      path: '/',
      query: { searchQuery: 'Test2' },
    });
    await router.isReady();

    const wrapper = mount(SearchBar, {
      global: {
        plugins: [router],
      },
    });

    const input = wrapper.find('input');
    await flushPromises();

    expect(input.element.value).toBe('Test2');

    const searchStore = useSearchStore('test');
    const symbol = Symbol('searchStoreSym');

    mount(SearchBar, {
      global: {
        plugins: [createTestingPinia(), router],
        provide: {
          [symbol as symbol]: searchStore,
        },
      },
      propsData: { searchStoreSymbol: symbol },
    });

    expect(searchStore.getSearchQuery).toBe('Test2');
  });
});
