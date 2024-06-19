import { mount } from '@vue/test-utils';
import { describe, it, expect, vi } from 'vitest';
import SearchBar from '../SearchBar.vue';
import { searchProjects } from '@/services/SearchService.ts';

vi.mock('../../../services/SearchService', () => ({
  searchProjects: vi.fn(() => Promise.resolve([])),
}));

describe('SearchBar.vue', () => {
  beforeEach(() => {
    // Reset mock before each test
    vi.clearAllMocks();
  });

  it('renders correctly', () => {
    const wrapper = mount(SearchBar);
    expect(wrapper.exists()).toBe(true);
  });

  it('calls searchProjects when the user provides input', async () => {
    const wrapper = mount(SearchBar);
    const input = wrapper.find('input');
    await input.setValue('Test');

    // wait for all asynchronous calls to complete
    await wrapper.vm.$nextTick();
    await wrapper.vm.$nextTick();

    expect(searchProjects).toHaveBeenCalled();
  });
  it('binds input value correctly to v-model', async () => {
    const wrapper = mount(SearchBar);
    const input = wrapper.find('input');
    await input.setValue('Test');

    expect(input.element.value).toBe('Test');
  });
});
