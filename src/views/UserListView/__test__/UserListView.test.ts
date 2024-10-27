import { mount } from '@vue/test-utils';
import { describe, it, expect } from 'vitest';
import UserListView from '@/views/UserListView/UserListView.vue';

describe('UserListView.vue', () => {
  it('renders correctly', async () => {
    const wrapper = mount(UserListView);
    expect(wrapper.find('.layout').exists()).toBe(true);
    expect(wrapper.find('.sideSlider').exists()).toBe(true);
    expect(wrapper.find('.menuItem').exists()).toBe(true);
    const user = await wrapper.findAll('.user');
    expect(user[0].exists()).toBe(true);
    expect(user[1].exists()).toBe(true);
    expect(user[2].exists()).toBe(true);
  });
});
