import { describe, it, expect } from 'vitest';
import { mount } from '@vue/test-utils';
import GroupedCard from '@/components/GroupedCard/GroupedCard.vue';

describe('GroupedCard.vue', () => {
  it('renders correctly with provided props', () => {
    const wrapper = mount(GroupedCard, {
      props: {
        pluginCount: 5,
        displayName: 'Test Plugin',
        faviconUrl: 'https://example.com/favicon.ico',
      },
    });

    console.log(wrapper.html()); // Debuging to see what is rendered

    // Checking if `h3` exists
    const heading = wrapper.find('h3');
    expect(heading.exists()).toBe(true);
    expect(heading.text()).toBe('Test Plugin Plugins');

    // Checking if `a-badge` exists
    const badge = wrapper.findComponent({ name: 'a-badge' });
    expect(badge.exists()).toBe(true);
    expect(badge.props('count')).toBe(5);

    // Checking if `a-avatar` exists
    const avatar = wrapper.findComponent({ name: 'a-avatar' });
    expect(avatar.exists()).toBe(true);
    expect(avatar.props('src')).toBe('https://example.com/favicon.ico');
  });

  it('emits "open" event when clicked', async () => {
    const wrapper = mount(GroupedCard, {
      props: {
        pluginCount: 5,
        displayName: 'Test Plugin',
        faviconUrl: 'https://example.com/favicon.ico',
      },
    });

    // Finding and clicking the clickable element
    const clickableElement = wrapper.find('.grouped-card');
    expect(clickableElement.exists()).toBe(true);
    await clickableElement.trigger('click');

    // Checking if the `open` event was emitted
    expect(wrapper.emitted()).toHaveProperty('open');
  });
});
