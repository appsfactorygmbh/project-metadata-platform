import { mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';
import UserInformationView from '../UserInformationView.vue';
import { createTestingPinia } from '@pinia/testing';
import { userRoutingSymbol, userStoreSymbol } from '@/store/injectionSymbols';
import { useUserStore } from '@/store';
import router from '@/router';
import type { UserModel } from '@/models/User';
import { EditOutlined } from '@ant-design/icons-vue';
import { useUserRouting } from '@/utils/hooks';

const userData1: UserModel = {
  id: '100',
  email: 'maxmuster1@gmail.com',
};
const userData2: UserModel = {
  id: '200',
  email: 'maxmuster2@gmail.com',
};

describe('UserInformationView.vue', () => {
  setActivePinia(createPinia());
  const userStore = useUserStore();

  const generateWrapper = () => {
    return mount(UserInformationView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        provide: {
          [userStoreSymbol as symbol]: userStore,
          [userRoutingSymbol as symbol]: useUserRouting(router),
        },
        plugins: [router],
      },
    });
  };

  it('renders correctly', () => {
    userStore.setMe(userData1);
    userStore.setUser(userData1);
    const wrapper = generateWrapper();

    expect(wrapper.find('.avatar').exists()).toBe(true);
    expect(wrapper.find('.label').exists()).toBe(true);
    const text = wrapper.findAll('.text');
    expect(text[0].text()).toBe(userData1.email);

    const button = wrapper.findAllComponents(EditOutlined);
    expect(button[0].exists()).toBe(true);
    expect(button[1].exists()).toBe(true);
    expect(wrapper.find('.email').exists()).toBe(true);
  });

  it('should show password', () => {
    userStore.setMe(userData1);
    userStore.setUser(userData1);
    const wrapper = generateWrapper();
    const textField = wrapper.find('.passwordLabel');
    expect(textField.exists()).toBe(true);
  });

  it('should hide password', () => {
    userStore.setMe(userData2);
    userStore.setUser(userData1);
    const wrapper = generateWrapper();
    const textField = wrapper.find('.passwordLabel');
    expect(textField.exists()).toBe(false);
  });
});
