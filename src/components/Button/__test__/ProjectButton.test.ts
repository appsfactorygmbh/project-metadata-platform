import { mount } from '@vue/test-utils';
import ProjectButton from '@/components/Button/ProjectButton/ProjectButton.vue';
import { describe, expect, it } from 'vitest';

describe('ProjectButton.vue', () => {
  it('renders the default button correctly', () => {
    const wrapper = mount(ProjectButton, {
      slots: {
        icon: '<span class="icon-test">Test Icon</span>',
      },
    });

    // Check the button exists
    expect(wrapper.find('button').exists()).toBe(true);

    // Check the slot content
    expect(wrapper.find('.icon-test').exists()).toBe(true);
    expect(wrapper.find('.icon-test').text()).toBe('Test Icon');
  });

  it('renders with custom props', () => {
    const wrapper = mount(ProjectButton, {
      props: {
        type: 'primary',
        shape: 'round',
        size: 'small',
      },
      slots: {
        icon: '<span class="icon-test">Custom Icon</span>',
      },
    });

    // Check that the props are passed correctly
    const button = wrapper.find('button');
    expect(button.classes()).toContain('button');
    expect(wrapper.props('type')).toBe('primary');
    expect(wrapper.props('shape')).toBe('round');
    expect(wrapper.props('size')).toBe('small');
  });

  it('emits click event when clicked', async () => {
    const wrapper = mount(ProjectButton);

    // Trigger a click
    await wrapper.trigger('click');

    // Verify the click event is emitted
    expect(wrapper.emitted('click')).toBeTruthy();
  });

  it('applies custom styles to the button', () => {
    const wrapper = mount(ProjectButton, {
      props: {
        type: 'ghost',
      },
      attrs: {
        style: 'background-color: red;',
      },
    });

    // Ensure custom styles are applied
    const button = wrapper.find('button');
    expect(button.attributes('style')).toContain('background-color: red;');
  });

  it('handles missing icon slot gracefully', () => {
    const wrapper = mount(ProjectButton);

    // Check the button exists without an icon
    const button = wrapper.find('button');
    expect(button.exists()).toBe(true);

    // Ensure there is no slot content
    expect(wrapper.html()).not.toContain('<slot name="icon"></slot>');
  });
});
