import MenuButtons from '../MenuButtons.vue';
import { mount } from '@vue/test-utils';
import { describe, it, expect } from 'vitest';

describe('MenuButtons.vue', () => {
  it('renders correctly', () => {
    const wrapper = mount(MenuButtons);
    const buttons = wrapper.findAll('.button');

    expect(buttons).toHaveLength(4);
    for (let i = 0; i < buttons.length; i++) {
      expect(buttons[i].isVisible).toBeTruthy();
    }
  });
});
