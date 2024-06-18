import { describe, it, expect } from 'vitest';
import { mount } from '@vue/test-utils';
import { createPinia } from 'pinia';
import App from '../../../App.vue';
import projectView from '../projectView.vue';

createApp(App).use(createPinia());

describe('projectView.vue', () => {
  it('displays the project name when not editing', async () => {
    const wrapper = mount(projectView, {
      propsData: {
        paneWidth: 1000,
      },
    });
    expect(wrapper.find('.projectNameH1').exists()).toBe(true);
    expect(wrapper.find('.projectNameInput').exists()).toBe(false);
    expect(wrapper.find('.projectNameH1').text()).toBe('');
  });

  it('toggles editing mode on edit button click', async () => {
    const wrapper = mount(projectView, {
      propsData: {
        paneWidth: 1000,
      },
    });
    const editButton = wrapper.find('.edit-button');
    await editButton.trigger('click');

    expect(wrapper.find('.projectNameH1').exists()).toBe(false);
    expect(wrapper.find('.projectNameInput').exists()).toBe(true);

    const input = wrapper.find('.projectNameInput');
    await input.setValue('Your Project Name');
  });

  it('Save name', async () => {
    const wrapper = mount(projectView, {
      propsData: {
        paneWidth: 1000,
      },
    });
    const editButton = wrapper.find('.edit-button');
    const input = wrapper.find('.projectNameInput');

    await editButton.trigger('click');
    expect(wrapper.find('.projectNameH1').text()).toBe('Your Project Name');
    expect((input.element as HTMLInputElement).value).toBe('Your Project Name');
  });
});
