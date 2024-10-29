import { flushPromises, mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';
import UserInformationView from '@/views/UserInformationView/UserInformationView.vue';
import { createTestingPinia } from '@pinia/testing';
import { userStoreSymbol } from '@/store/injectionSymbols';
import { useUserStore } from '@/store';

const userData = {
  id: 100,
  name: 'Max Mustermann',
  username: 'Maxmuster',
  email: 'maxmuster@gmail.com',
};

describe('UserInformationView.vue', () => {
  setActivePinia(createPinia());

  it('renders correctly', async () => {
    const wrapper = mount(UserInformationView, {
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
      },
    });
    await flushPromises();

    expect(wrapper.find('.avatar').exists()).toBe(true);
    expect(wrapper.find('.label').exists()).toBe(true);
    const text = await wrapper.findAll('.text');
    expect(text[0].text()).toBe('Max Mustermann');
    expect(text[1].text()).toBe('Maxmuster');
    expect(text[2].text()).toBe('maxmuster@gmail.com');

    const button = await wrapper.findAll('.edit');
    expect(button[0].exists()).toBe(true);
    expect(button[1].exists()).toBe(true);
    expect(button[2].exists()).toBe(true);
    expect(button[3].exists()).toBe(true);
  });
});
