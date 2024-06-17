import { describe, it, expect } from 'vitest';
import { mount } from '@vue/test-utils';
import SplitView from '../SplitView.vue';

createApp(App).use(createPinia());

describe('App.vue', () => {
  it('renders correctly', () => {
    const wrapper = mount(SplitView, {
      attachTo: document.body,
    });

    expect(wrapper.findAll('.splitpanes__pane')[0].isVisible()).toBe(true);
    expect(wrapper.findAll('.splitpanes__pane')[1].isVisible()).toBe(true);
  });
});
