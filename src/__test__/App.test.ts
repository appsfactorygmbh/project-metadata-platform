import { describe, it, expect } from 'vitest';
import { mount } from '@vue/test-utils';
import { createPinia } from 'pinia';
import App from '../App.vue';

createApp(App).use(createPinia());

describe('App.vue', () => {
  it('renders correctly', () => {
    const wrapper = mount(App, {
      global: {
        stubs: {
          Table: {
            template: '<span />',
          },
        },
      },
    });

    expect(wrapper.findAll('.splitpanes__pane')[0].isVisible()).toBe(true);
    expect(wrapper.findAll('.splitpanes__pane')[1].isVisible()).toBe(true);
  });
});
