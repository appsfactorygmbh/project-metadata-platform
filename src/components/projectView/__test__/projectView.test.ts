import { describe, it, expect } from 'vitest';
import { mount, VueWrapper } from '@vue/test-utils';
import projectView from '../projectView.vue';

describe('projectView.vue', () => {
  let wrapper: VueWrapper<boolean>;
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
    const editButton = wrapper.find('.edit-button');
    await editButton.trigger('click');
    expect(wrapper.find('h1').exists()).toBe(false);
    expect((wrapper.vm as any).isEditing).toBe(true);
  });
});