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
  userName: 'maxmuster1@gmail.com',
  externalId: '100',
  active: false,
  addresses: [{ locality: 'Leipzig' }],
  urnIetfParamsScimSchemasExtensionEnterprise20User: {
    organization: 'MusterCorp',
  },
  urnIetfParamsScimSchemasExtensionPmpUser: {
    departments: ['IT', 'TI'],
    businessUnits: ['Internal'],
    team: ['Team 5000'],
    teamSupport: ['Team 9001'],
  },
  meta: {},
};
const userData2: UserModel = {
  id: '200',
  userName: 'maxmuster2@gmail.com',
  externalId: '200',
  active: false,
  addresses: [],
  urnIetfParamsScimSchemasExtensionEnterprise20User: {},
  urnIetfParamsScimSchemasExtensionPmpUser: {},
  meta: {},
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
    expect(text[0].text()).toBe(userData1.externalId);
    expect(text[1].text()).toBe(userData1.userName);
    expect(text[3].text()).toBe('');
    expect(text[4].text()).toBe('Team 5000');
    expect(text[5].text()).toBe('Team 9001');
    expect(text[6].text()).toBe('IT, TI');
    expect(text[7].text()).toBe('Internal');
    expect(text[8].text()).toBe('Leipzig');
    expect(text[9].text()).toBe('MusterCorp');

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
    const textField = wrapper.findAll('.label')[2];
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
