import { flushPromises, mount } from '@vue/test-utils';
import { describe, expect, it, vi } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { userRoutingSymbol, userStoreSymbol } from '@/store/injectionSymbols';
import { useUserStore } from '@/store';
import { PlusOutlined } from '@ant-design/icons-vue';
import { UserListView } from '..';

vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
  }),
}));

const userData1 = {
  userName: 'maxmuster1@gmail.com',
  externalId: '100',
  isScimProvisioned: false,
};
const userData2 = {
  userName: 'maxmuster2@gmail.com',
  externalId: '200',
  isScimProvisioned: true,
};

describe('UserListView.vue', () => {
  const generateWrapper = () => {
    const pinia = createTestingPinia({
      stubActions: true,
      initialState: {
        user: { users: [userData1, userData2], isLoading: false },
      },
    });

    const userStore = useUserStore();

    const mockUserRouting = {
      routerUserId: ref(''),
      setUserId: vi.fn(),
    };

    return mount(UserListView, {
      global: {
        plugins: [pinia],
        components: {
          PlusOutlined,
        },
        stubs: {
          RouterView: true,
        },
        provide: {
          [userStoreSymbol as symbol]: userStore,
          [userRoutingSymbol as symbol]: mockUserRouting,
        },
      },
    });
  };

  it('renders correctly', async () => {
    const wrapper = generateWrapper();

    await flushPromises();

    expect(wrapper.find('.layout').exists()).toBe(true);

    const icon = wrapper.findComponent(PlusOutlined);
    expect(icon.exists()).toBe(true);

    expect(wrapper.text()).toContain('Create New User');

    expect(wrapper.text()).toContain('maxmuster1');
    expect(wrapper.text()).toContain('maxmuster2');

    const tags = wrapper.findAll('.scim-tag');
    expect(tags.length).toBe(1);
    expect(tags[0].text()).toBe('SCIM');
  });

  it('calls fetchMe and fetchAll on mount', () => {
    generateWrapper();
    const userStore = useUserStore();

    expect(userStore.fetchMe).toHaveBeenCalled();
    expect(userStore.fetchAll).toHaveBeenCalled();
  });
});
