import { describe, it, expect, vi } from 'vitest';
import { shallowMount } from '@vue/test-utils';
import Popup from '@/components/Popup/Popup.vue';
import { PluginComponent } from '@/components/Plugin';

describe('Popup.vue', () => {
  const selectedGroupMock = {
    pluginName: 'Test Group',
    plugins: [
      { id: 1, pluginName: 'Plugin 1', displayName: 'Plugin 1', url: 'https://example.com' },
      { id: 2, pluginName: 'Plugin 2', displayName: 'Plugin 2', url: 'https://example.com' }
    ]
  };

  it('renders correctly when selectedGroup is provided', () => {
    const wrapper = shallowMount(Popup, {
      props: {
        selectedGroup: selectedGroupMock,
        loading: false,
        isEditing: false
      }
    });

    // Check if it renders the group name correctly
    expect(wrapper.find('h3').text()).toBe('Plugins in Test Group');

    // We check that it renders the correct number of PluginComponent components
    const pluginComponents = wrapper.findAllComponents(PluginComponent);
    expect(pluginComponents.length).toBe(2);
  });

  it('emits "close" event when close button is clicked', async () => {
    const wrapper = shallowMount(Popup, {
      props: {
        selectedGroup: selectedGroupMock
      }
    });

    // Finds the close button and clicks it
    await wrapper.find('a-button').trigger('click');

    // Check if the “close” event has been emitted
    expect(wrapper.emitted()).toHaveProperty('close');
  });
});
