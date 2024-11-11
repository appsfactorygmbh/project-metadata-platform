import { mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';
import UserInformationView from '../UserInformationView.vue';
import { createTestingPinia } from '@pinia/testing';
import { userStoreSymbol } from '@/store/injectionSymbols';
import { useUserStore } from '@/store';
import router from '@/router';

const userData = {
  id: 100,
  name: 'Max Musterfrau',
  username: 'Maxmuster1',
  email: 'maxmuster1@gmail.com',
};

describe('UserInformationView.vue', () => {
  setActivePinia(createPinia());

  const generateWrapper = () => {
    return mount(UserInformationView, {
      plugins: [
        createTestingPinia({
          stubActions: true,
          initialState: {
            user: {
              user: userData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [userStoreSymbol as symbol]: useUserStore(),
        },
        plugins: [router],
      },
    });
  };

  it('renders correctly', () => {
    const wrapper = generateWrapper();

    expect(wrapper.find('.avatar').exists()).toBe(true);
    expect(wrapper.find('.label').exists()).toBe(true);
    const text = wrapper.findAll('.text');
    expect(text[0].text()).toBe(userData.name);
    expect(text[1].text()).toBe(userData.username);
    expect(text[2].text()).toBe(userData.email);

    const button = wrapper.findAll('.edit');
    expect(button[0].exists()).toBe(true);
    expect(button[1].exists()).toBe(true);
    expect(button[2].exists()).toBe(true);
    expect(wrapper.find('.name').exists()).toBe(true);
  });
});
