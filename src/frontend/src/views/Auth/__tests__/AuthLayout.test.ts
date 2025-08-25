import { describe, expect, it } from 'vitest';
import { mount } from '@vue/test-utils';
import { AuthLayout } from '..';

describe('AuthLayout.vue', () => {
  it('renders correctly', () => {
    const wrapper = mount(AuthLayout);

    expect(wrapper.exists()).toBe(true);
  });
});
