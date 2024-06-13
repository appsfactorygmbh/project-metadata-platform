import { describe, it, expect } from 'vitest';
import { mount } from '@vue/test-utils';
import { createPinia } from 'pinia';
import App from '../App.vue';

describe('App.vue', () => {
  it('renders correctly', () => {
    createApp(App).use(createPinia());
    const wrapper = mount(App);

    expect(wrapper.findAll('.splitpanes__pane')[0].isVisible()).toBe(true);
    expect(wrapper.findAll('.splitpanes__pane')[1].isVisible()).toBe(true);
  });
});
