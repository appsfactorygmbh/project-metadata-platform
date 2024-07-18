import { mount, flushPromises } from '@vue/test-utils';
import { describe, it, expect, vi } from 'vitest';
import SearchBar from '../SearchBar.vue';
import { useSearchStore } from '@/store';
import { createPinia, setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';

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
});
