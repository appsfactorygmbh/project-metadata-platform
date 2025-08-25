import { mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import { userRoutingSymbol, userStoreSymbol } from '@/store/injectionSymbols';
import { useUserStore } from '@/store';
import router from '@/router';
import type { UserModel } from '@/models/User';
import { DeleteOutlined, EditOutlined } from '@ant-design/icons-vue';
import { useUserRouting } from '@/utils/hooks';
import { UserInformationView } from '..';

const userData1: UserModel = {
  id: '100',
  email: 'maxmuster1@gmail.com',
};
const userData2: UserModel = {
  id: '200',
  email: 'maxmuster2@gmail.com',
};

const mockRoute = {
  path: '/mock-path',
  query: { userId: '200' },
  params: {},
  hash: '',
  fullPath: '/mock-path',
  matched: [],
  meta: {},
  redirectedFrom: undefined,
};
const mockRouter = {
  push: vi.fn(),
};

vi.mock('vue-router', async (importOriginal) => {
  const actual = await importOriginal<typeof import('vue-router')>();
  return {
    ...actual,
    useRoute: () => mockRoute,
    useRouter: () => mockRouter,
  };
});

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
          [userRoutingSymbol as symbol]: useUserRouting(),
        },
        plugins: [router],
      },
    });
  };

  it('should hide delete button when user is me', async () => {
    userStore.setMe(userData2);
    userStore.setUser(userData2);
    const wrapper = generateWrapper();
    const deleteButton = wrapper.findComponent(DeleteOutlined);
    expect(deleteButton.exists()).toBe(false);
  });

  it('should show delete button when user is not me', () => {
    userStore.setMe(userData1);
    userStore.setUser(userData2);
    const wrapper = generateWrapper();

    const deleteButton = wrapper.findComponent(DeleteOutlined);
    expect(deleteButton.exists()).toBe(true);
  });

  it('renders correctly', () => {
    mockRoute.query.userId = '100';
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
    mockRoute.query.userId = '100';
    userStore.setMe(userData1);
    userStore.setUser(userData1);
    const wrapper = generateWrapper();
    const textField = wrapper.findAll('.label')[1];
    expect(textField.text()).toBe('Password:');
  });

  it('should hide password', () => {
    mockRoute.query.userId = '100';
    userStore.setMe(userData2);
    userStore.setUser(userData1);
    const wrapper = generateWrapper();
    const textField = wrapper.find('.passwordLabel');
    expect(textField.exists()).toBe(false);
  });
});
