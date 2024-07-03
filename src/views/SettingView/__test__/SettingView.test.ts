import { mount } from '@vue/test-utils';
import { describe, it, expect } from 'vitest';
import SettingView from '@/views/SettingView/SettingView.vue'; // Adjust the path as necessary
interface SettingViewObject {
  collapsed: boolean;
  selectedKeys: string[];
  tab: string;
}

describe('SettingView.vue', () => {
  it('renders correctly', () => {
    const wrapper = mount(SettingView);
    expect(wrapper.find('.layout').exists()).toBe(true);
    expect(wrapper.find('.sideSlider').exists()).toBe(true);
    expect(wrapper.find('.menuItem').exists()).toBe(true);
    expect(wrapper.find('.addressBar').exists()).toBe(true);
  });

  it('has initial state', () => {
    const wrapper = mount(SettingView);
    const vm = wrapper.vm as unknown as SettingViewObject;
    expect(vm.collapsed).toBe(false);
    expect(vm.selectedKeys).toEqual(['1']);
    expect(vm.tab).toBe('');
  });

  it('updates tab on menu item click', async () => {
    const wrapper = mount(SettingView);
    const vm = wrapper.vm as unknown as SettingViewObject;

    await wrapper.find('.item2').trigger('click');
    expect(vm.tab).toBe('Plugin Creation');

    await wrapper.find('.item3').trigger('click');
    expect(vm.tab).toBe('Global Logs');
  });

  /*it('calls placeholder function on breadcrumb click', async () => {
    const wrapper = mount(SettingView);
    const consoleMock = vi.spyOn(console, 'log').mockImplementation(() => undefined);

    // Simulate click on the breadcrumb item
    await wrapper.find('.breadcrumbItem').trigger('click');
    expect(consoleMock).toHaveBeenCalled();

    // Assert that console.log was called with 'Icon clicked'

    expect(consoleMock.mock.calls).toContainEqual(['Icon clicked']);


  });*/
});
