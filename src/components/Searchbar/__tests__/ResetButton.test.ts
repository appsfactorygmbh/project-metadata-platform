import { mount } from '@vue/test-utils';
import { describe, it, expect, vi } from 'vitest';
import ResetButton from '../ResetButton.vue';
import { createTestingPinia } from '@pinia/testing';

describe('SearchBar.vue', () => {
  createTestingPinia();
  beforeEach(() => {
    // Reset mock before each test
    vi.clearAllMocks();
  });

  it('renders correctly', async () => {
    const wrapper = mount(ResetButton);
    expect(wrapper.exists()).toBe(true);
    expect(wrapper.find('.reset').exists()).toBe(true);
  });
});
