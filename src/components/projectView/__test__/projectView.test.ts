import { describe, it, expect } from 'vitest';
import { mount } from '@vue/test-utils';
import projectView from '../projectView.vue';

describe('projectView.vue', () => {
  it('displays the project name when not editing', async () => {
    const wrapper = mount(projectView, {
      data() {
        return {
          isEditing: false,
          projectName: 'Your Project Name'
        };
      }
    });

    expect(wrapper.find('h1').exists()).toBe(true);
    expect((wrapper.vm as any).isEditing).toBe(false);
  });

  it('toggles editing mode on edit button click', async () => {
    const wrapper = mount(projectView);

    // Find and click the edit button
    const editButton = wrapper.find('.edit-button');
    await editButton.trigger('click');

    expect(wrapper.find('h1').exists()).toBe(false);
    expect((wrapper.vm as any).isEditing).toBe(true);

    // You can perform additional assertions based on your component's behavior
  });
});

