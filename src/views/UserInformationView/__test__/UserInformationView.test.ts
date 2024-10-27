import { mount } from '@vue/test-utils';
import { describe, it, expect } from 'vitest';
import UserInformationView from '@/views/UserInformationView/UserInformationView.vue';

describe('UserInformationView.vue', () => {
  it('renders correctly', async () => {
    const wrapper = mount(UserInformationView);
    expect(wrapper.find('.avatar').exists()).toBe(true);
    expect(wrapper.find('.label').exists()).toBe(true);
    const text = await wrapper.findAll('.text');
    expect(text[0].exists()).toBe(true);
    expect(text[1].exists()).toBe(true);
    expect(text[2].exists()).toBe(true);
    expect(text[3].exists()).toBe(true);
    const button = await wrapper.findAll('.edit');
    expect(button[0].exists()).toBe(true);
    expect(button[1].exists()).toBe(true);
    expect(button[2].exists()).toBe(true);
    expect(button[3].exists()).toBe(true);
  });
});
