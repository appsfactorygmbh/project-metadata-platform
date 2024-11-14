import { describe, it, expect } from 'vitest';
import { mount } from '@vue/test-utils';
import Popup from '@/components/Popup/PopupComponent.vue';
import { PluginComponent } from '@/components/Plugin';

describe('Popup.vue', () => {
  const selectedGroupMock = {
    pluginName: 'Test Group',
    plugins: [
      {
        id: 1,
        pluginName: 'Plugin 1',
        displayName: 'Plugin 1',
        url: 'https://example.com',
      },
      {
        id: 2,
        pluginName: 'Plugin 2',
        displayName: 'Plugin 2',
        url: 'https://example.com',
      },
    ],
  };

  it('renders correctly when selectedGroup is provided', () => {
    const wrapper = mount(Popup, {
      props: {
        selectedGroup: selectedGroupMock,
        loading: false,
        isEditing: false,
      },
    });

    console.log(wrapper.html()); // Debuging to see what is rendered

    // Checking if `h3` exists
    const heading = wrapper.find('h3');
    expect(heading.exists()).toBe(true);
    expect(heading.text()).toBe('Plugins in Test Group');

    // Checking if `PluginComponent` is rendered for each plugin
    const pluginComponents = wrapper.findAllComponents(PluginComponent);
    expect(pluginComponents.length).toBe(2);
  });

  it('emits "close" event when close button is clicked', async () => {
    const wrapper = mount(Popup, {
      props: {
        selectedGroup: selectedGroupMock,
      },
    });

    // Finding and clicking the close button
    const closeButton = wrapper.find('button');
    expect(closeButton.exists()).toBe(true);
    await closeButton.trigger('click');

    // Checking if the `close` event was emitted
    expect(wrapper.emitted()).toHaveProperty('close');
  });
});
