import { mount } from '@vue/test-utils';
import { describe, expect, it, vi } from 'vitest';
import SettingView from '@/views/SettingView/SettingView.vue';
import {
  type RouteRecordRaw,
  type Router,
  createRouter,
  createWebHistory,
} from 'vue-router';

interface actualRouter {
  createRouter: typeof createRouter;
  createWebHistory: typeof createWebHistory;
  useRouter: typeof useRouter;
}

// Mock the entire vue-router module
vi.mock('vue-router', async () => {
  const actual: actualRouter = await vi.importActual('vue-router');
  const mockRouter: Router = {
    ...actual.createRouter({
      history: actual.createWebHistory(),
      routes: [],
    }),
    push: vi.fn(),
  } as Router;
  return {
    ...actual,
    useRouter: () => mockRouter,
    createRouter: vi.fn().mockImplementation(() => {
      return mockRouter;
    }),
  };
});

interface SettingViewObject {
  collapsed: boolean;
  selectedKeys: string[];
  tab: string;
  clickTab: (name: string) => void;
  goToMain: () => void;
}

describe('SettingView.vue', () => {
  let mockRouter: Router;

  beforeEach(() => {
    const MockComponent = { template: '<div></div>' };
    const routes: RouteRecordRaw[] = [
      { path: '/', name: 'Home', component: MockComponent },
      {
        path: '/settings/user-management',
        name: 'UserManagement',
        component: MockComponent,
      },
      {
        path: '/settings/global-plugins',
        name: 'GlobalPlugins',
        component: MockComponent,
      },
      {
        path: '/settings/global-logs',
        name: 'GlobalLogs',
        component: MockComponent,
      },
    ];

    mockRouter = createRouter({
      history: createWebHistory(),
      routes,
    });
  });

  it('renders correctly', () => {
    const wrapper = mount(SettingView, {
      global: {
        plugins: [mockRouter],
      },
    });
    expect(wrapper.find('.layout').exists()).toBe(true);
    expect(wrapper.find('.sideSlider').exists()).toBe(true);
    expect(wrapper.find('.menuItem').exists()).toBe(true);
    expect(wrapper.find('.addressBar').exists()).toBe(true);
    expect(wrapper.find('.backButton button').exists()).toBe(true);
  });

  it('has initial state', () => {
    const wrapper = mount(SettingView, {
      global: {
        plugins: [mockRouter],
      },
    });
    const vm = wrapper.vm as unknown as SettingViewObject;
    expect(vm.collapsed).toBe(false);
    expect(vm.tab).toBe('Global Plugins');
  });

  it('go to main menu when back click', async () => {
    const wrapper = mount(SettingView, {
      global: {
        plugins: [mockRouter],
      },
    });
    await wrapper.find('.backButton button').trigger('click');

    expect(mockRouter.push).toHaveBeenCalledWith('/');
  });

  it('go to other tab when tab click', async () => {
    const wrapper = mount(SettingView, {
      global: {
        plugins: [mockRouter],
      },
    });
    await wrapper.find('.userManagement').trigger('click');
    expect(mockRouter.push).toHaveBeenCalledWith('/settings/user-management');

    await wrapper.find('.globalLogs').trigger('click');
    expect(mockRouter.push).toHaveBeenCalledWith('/settings/global-logs');
  });
});
