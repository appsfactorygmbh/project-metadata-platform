import { describe, it, expect, vi } from 'vitest';
import { mount } from '@vue/test-utils';
import { createPinia } from 'pinia';
import App from '../../../App.vue';
import ProjectInformation from '../ProjectInformation.vue';

createApp(App).use(createPinia());

const data = {
  projectName: 'Heute Show',
  department: 'IT',
  clientName: 'Zdf',
  businessUnit: 'Bu Health',
  teamNumber: 42,
};

describe('projectView.vue', () => {
  const mockResponse = {
    ok: true,
    statusText: 'Ok',
    json: async () => data,
  } as Response;
  globalThis.fetch = vi.fn().mockResolvedValue(mockResponse);

  it('displays the project name when not editing', async () => {
    const wrapper = mount(ProjectInformation, {
      propsData: {
        paneWidth: 1000,
        isTest: true,
      },
    });
    expect(wrapper.find('.projectNameH1').exists()).toBe(true);

    expect(wrapper.find('.projectNameInput').exists()).toBe(false);
    expect(wrapper.find('.projectNameH1').text()).toBe('');
  });

  it('toggles editing mode on edit button click', async () => {
    const wrapper = mount(ProjectInformation, {
      propsData: {
        paneWidth: 1000,
      },
    });
    const editButton = wrapper.find('.edit-button');
    await editButton.trigger('click');

    expect(wrapper.find('.projectNameH1').exists()).toBe(false);
    expect(wrapper.find('input').exists()).toBe(true);
  });

  it('Save name', async () => {
    const wrapper = mount(ProjectInformation, {
      propsData: {
        paneWidth: 1000,
      },
    });
    const editButton = wrapper.find('.edit-button');
    const input = wrapper.find('.projectNameInput');

    await editButton.trigger('click');
    expect(wrapper.find('.projectNameH1').text()).toBe('Heute Show');
    expect((input.element as HTMLInputElement).value).toBe('Heute Show');
  });
});
