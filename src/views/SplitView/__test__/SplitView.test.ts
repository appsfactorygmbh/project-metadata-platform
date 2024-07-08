import { describe, it, expect } from 'vitest';
import { mount } from '@vue/test-utils';
import SplitView from '../SplitView.vue';
import { createPinia, setActivePinia } from 'pinia';

setActivePinia(createPinia());

describe('SplitView.vue', () => {
  it('renders correctly', () => {
    const wrapper = mount(SplitView, {
      global: {
        stubs: {
          ProjectSearchView: {
            template: '<span />',
          },
          ProjectView: {
            template: '<span />',
          },
        },
      },
    });

    expect(wrapper.findAll('.splitpanes__pane')[0].isVisible()).toBeTruthy();
    expect(wrapper.findAll('.splitpanes__pane')[1].isVisible()).toBeTruthy();
  });
});
