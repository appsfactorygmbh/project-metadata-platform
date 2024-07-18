import { describe, expect, it } from 'vitest';
import { mount } from '@vue/test-utils';
import { RegisterView } from '../';

describe('RegisterView.vue', () => {
  it('renders correctly', () => {
    const wrapper = mount(RegisterView);

    expect(wrapper.exists()).toBe(true);
  });
});
