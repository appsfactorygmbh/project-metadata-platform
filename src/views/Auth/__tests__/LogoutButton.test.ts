import { describe, expect, it } from 'vitest';
import { mount } from '@vue/test-utils';
import { LogoutButton } from '../';

describe('LogoutButton.vue', () => {
  it('renders correctly', () => {
    const wrapper = mount(LogoutButton);

    expect(wrapper.exists()).toBe(true);
  });
});
