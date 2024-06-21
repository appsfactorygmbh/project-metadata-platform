import { describe, it, expect, vi } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import { createPinia, setActivePinia } from 'pinia';
import ProjectInformation from '../ProjectInformation.vue';

setActivePinia(createPinia());

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
    await flushPromises();
    console.log(wrapper.html());

    expect(wrapper.find('.projectName').exists()).toBe(true);

    expect(wrapper.find('.projectNameInput').exists()).toBe(false);
    expect(wrapper.find('.projectName').text()).toBe('');
  });

  it('Save name', async () => {
    const wrapper = mount(ProjectInformation, {
      propsData: {
        paneWidth: 1000,
        isTest: true,
      },
    });
    await flushPromises();
    expect(wrapper.find('.projectNameContainer').text()).toBe('Heute Show');
  });
});
