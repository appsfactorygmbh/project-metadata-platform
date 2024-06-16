import { describe, it, expect } from 'vitest';
import { mount } from '@vue/test-utils';
import App from '../App.vue';

describe('App.vue', () => {
  it('renders correctly', () => {
    const wrapper = mount(App, {
      attachTo: document.body,
    });

    expect(wrapper.findAll('.splitpanes__pane')[0].isVisible()).toBe(true);
    expect(wrapper.findAll('.splitpanes__pane')[1].isVisible()).toBe(true);
  });
});
