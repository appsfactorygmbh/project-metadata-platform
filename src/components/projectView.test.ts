import { describe, it, expect } from 'vitest';
import { mount } from '@vue/test-utils';
import projectView from './projectView.vue';

describe('projectView.vue', () => {
  const wrapper = mount(projectView);
  it('toggles editing mode', async () => {
    const editButton = wrapper.find('Edit]');
    await editButton.trigger('click');
    expect(wrapper.find('').exists()).toBe(true);

    const input = wrapper.find('input#projectNameInput');
    input.setValue('Updated Project Name');
    await editButton.trigger('click');
    expect(wrapper.find('h1').text()).toBe('Updated Project Name');
  });
});


