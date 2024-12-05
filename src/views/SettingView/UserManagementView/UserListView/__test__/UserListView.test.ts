import { mount } from '@vue/test-utils';
import UserListView from '../UserListView.vue';
import { describe, expect, it } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { useUserStore } from '@/store';
import { createPinia, setActivePinia } from 'pinia';
import { userStoreSymbol } from '@/store/injectionSymbols';
import router from '@/router';

interface UserListViewInstance {
  routerUser: number;
  clickTab: (id: number) => void;
}

const userData = [
  {
    id: 100,
    email: 'maxmuster1@gmail.com',
  },
  {
    id: 200,
    email: 'maxmuster2@gmail.com',
  },
];

const generateWrapper = () => {
  return mount(UserListView, {
    plugins: [
      createTestingPinia({
        stubActions: true,
        initialState: {
          user: {
            users: userData,
          },
        },
      }),
    ],
    global: {
      provide: {
        [userStoreSymbol as symbol]: useUserStore(),
      },
      plugins: [router],
      stubs: {
        'a-menu-item': {
          template: '<div class="users"><slot /></div>',
        },
      },
    },
  });
};

describe('UserListView.vue', () => {
  setActivePinia(createPinia());

  it('renders correctly', () => {
    const wrapper = generateWrapper();
    expect(wrapper.find('.layout').exists()).toBe(true);
    expect(wrapper.find('.sideSlider').exists()).toBe(true);
    const menuItems = wrapper.findAll('.users');
    expect(menuItems.length).toBe(userData.length);

    menuItems.forEach((itemWrapper, index) => {
      expect(itemWrapper.text()).toContain(userData[index].email.split('@')[0]);
    });
  });

  it('adds a query when clicking on a user', () => {
    const wrapper = generateWrapper();

    const vm = wrapper.vm as unknown as UserListViewInstance;
    vm.clickTab(userData[0].id);
    expect(vm.routerUser).toEqual(userData[0].id);
  });
});
