import { flushPromises, mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';
import UserInformationView from '@/views/UserInformationView/UserInformationView.vue';
import { createTestingPinia } from '@pinia/testing';
import { userStoreSymbol } from '@/store/injectionSymbols';
import { useUserStore } from '@/store';
import { useCurrentUserStore } from '@/store/CurrentUserStore';
interface UserInformationViewInstance {
  checkCurrentUser: () => boolean;
  isUser: boolean;
  test: () => string[];
}

const userData1 = {
  id: 100,
  name: 'Max Musterfrau',
  username: 'Maxmuster1',
  email: 'maxmuster1@gmail.com',
};

const userData2 = {
  id: 200,
  name: 'Max Mustermann',
  username: 'Maxmuster2',
  email: 'maxmuste2r@gmail.com',
};

describe('UserInformationView.vue', () => {
  setActivePinia(createPinia());
  const userStore = useUserStore();

  const generateWrapper = () => {
    return mount(UserInformationView, {
      plugins: [
        createTestingPinia({
          stubActions: true,
          initialState: {
            currentUser: {
              currentUser: { username: userData1.username },
            },
          },
        }),
      ],
      global: {
        provide: {
          [userStoreSymbol as symbol]: userStore,
          currentUserStore: useCurrentUserStore(),
        },
      },
    });
  };

  it('renders correctly', async () => {
    userStore.setUser(userData1);
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

  it('should show password', () => {
    const wrapper = generateWrapper();
    const vm = wrapper.vm as unknown as UserInformationViewInstance;

    const text = wrapper.findAll('.text');
    expect(text[3].exists()).toBe(true);
    expect(vm.isUser).toBe(true);
    expect(text[3].text()).toBe('Super Secret Password');
  });

  it('should hide password', () => {
    userStore.setUser(userData2);
    const wrapper = generateWrapper();
    const vm = wrapper.vm as unknown as UserInformationViewInstance;

    const text = wrapper.findAll('.text');
    expect(text[3].exists()).toBe(true);
    expect(vm.isUser).toBe(false);
    expect(text[3].text()).toBe('');
  });
});
