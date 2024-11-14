import { describe, it, expect } from 'vitest';
import { shallowMount } from '@vue/test-utils';
import GroupedCard from '@/components/GroupedCard/GroupedCard.vue';

describe('GroupedCard.vue', () => {
  it('renders correctly with provided props', () => {
    const wrapper = shallowMount(GroupedCard, {
      props: {
        pluginCount: 5,
        displayName: 'Test Plugin',
        faviconUrl: 'https://example.com/favicon.ico'
      }
    });

    // Check if it renders the group name correctly
    expect(wrapper.find('h3').text()).toBe('Test Plugin Plugins');

    // Check if the a-badge component received the correct value of `pluginCount`
    const badge = wrapper.findComponent({ name: 'a-badge' });
    expect(badge.props('count')).toBe(5);

    // Check if the faviconUrl image is set correctly
    const avatar = wrapper.findComponent({ name: 'a-avatar' });
    expect(avatar.props('src')).toBe('https://example.com/favicon.ico');
  });

  it('emits "open" event when clicked', async () => {
    const wrapper = shallowMount(GroupedCard, {
      props: {
        pluginCount: 5,
        displayName: 'Test Plugin',
        faviconUrl: 'https://example.com/favicon.ico'
      }
    });

    // Clicks at the main div-component
    await wrapper.trigger('click');

    // Check if the `open` event has been emitted.
    expect(wrapper.emitted()).toHaveProperty('open');
  });
});
