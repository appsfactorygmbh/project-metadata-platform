import MenuButtons from '../MenuButtons.vue';
import { mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';

describe('MenuButtons.vue', () => {
  setActivePinia(createPinia());

  it('renders correctly', () => {
    const wrapper = mount(MenuButtons);
    const buttons = wrapper.findAll('.ant-float-btn');

    expect(buttons).toHaveLength(3);
    for (let i = 0; i < buttons.length; i++) {
      expect(buttons[i].isVisible).toBeTruthy();
    }
  });
});
