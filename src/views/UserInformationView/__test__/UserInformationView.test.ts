import { flushPromises, mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';
import UserInformationView from '@/views/UserInformationView/UserInformationView.vue';
import { createTestingPinia } from '@pinia/testing';
import { userStoreSymbol } from '@/store/injectionSymbols';
import { useUserStore } from '@/store';
import router from '@/router';

const userData1 = {
  id: 100,
  name: 'Max Musterfrau',
  username: 'Maxmuster1',
  email: 'maxmuster1@gmail.com',
};

describe('UserInformationView.vue', () => {
  setActivePinia(createPinia());
  const userStore = useUserStore();

  const generateWrapper = () => {
    return mount(UserInformationView, {
      plugins: [
        createTestingPinia({
          stubActions: true,
        }),
      ],
      global: {
        provide: {
          [userStoreSymbol as symbol]: userStore,
        },
        plugins: [router],
      },
    });
  };

  it('renders correctly', async () => {
    userStore.setMe(userData1);
    const wrapper = generateWrapper();
    await flushPromises();

    expect(wrapper.find('.avatar').exists()).toBe(true);
    expect(wrapper.find('.label').exists()).toBe(true);
    const text = wrapper.findAll('.text');
    expect(text[0].text()).toBe(userData1.name);
    expect(text[1].text()).toBe(userData1.username);
    expect(text[2].text()).toBe(userData1.email);

    const button = wrapper.findAll('.edit');
    expect(button[0].exists()).toBe(true);
    expect(button[1].exists()).toBe(true);
    expect(button[2].exists()).toBe(true);
    expect(wrapper.find('.name').exists()).toBe(true);
  });
});
