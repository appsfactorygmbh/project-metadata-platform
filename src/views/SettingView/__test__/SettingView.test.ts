import { mount } from '@vue/test-utils';
import { describe, it, expect } from 'vitest';
import SettingView from '@/views/SettingView/SettingView.vue'; // Adjust the path as necessary
import { useRouter } from 'vue-router';
import router from '../../../router';

vi.mock('vue-router');

interface SettingViewObject {
  collapsed: boolean;
  selectedKeys: string[];
  tab: string;
  clickTab: (name: string) => void;
  goToMain: () => void;
}

describe('SettingView.vue', () => {
  vi.mocked(useRouter).mockReturnValue({
    ...router,
    push: vi.fn(),
  });

  beforeEach(() => {
    vi.mocked(useRouter().push).mockReset();
  });

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

  it('go to main menu when back click', async () => {
    const wrapper = mount(SettingView);
    await wrapper.find('.iconBack').trigger('click');

    expect(useRouter().push).toHaveBeenCalledWith('/');
  });

  it('go to other tab when tab click', async () => {
    const wrapper = mount(SettingView);
    await wrapper.find('.pluginCreation').trigger('click');
    expect(useRouter().push).toHaveBeenCalledWith('/settings/plugins');

    await wrapper.find('.globalLogs').trigger('click');
    expect(useRouter().push).toHaveBeenCalledWith('/settings/global-logs');
  });
});
