import { useAuthorizationStore } from '@/store';
import { createTestingPinia } from '@pinia/testing';
import { AuthorizationView } from '..';
import { authorizationStoreSymbol } from '@/store/injectionSymbols';
import { flushPromises, mount } from '@vue/test-utils';
import { ResourceActions } from '@/models/utils';

vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
  }),
}));
const permissionData = {
  action: ResourceActions.Get,
  filter: {
    value: 'AlwaysAllowed',
  },
};
describe('AuthorizationView.vue', () => {
  const generateWrapper = () => {
    const pinia = createTestingPinia({
      stubActions: true,
      initialState: {
        authorization: {
          resources: ['Project'],
          permissionFilters: [permissionData],
          isLoading: false,
        },
      },
    });

    const authorizationStore = useAuthorizationStore(pinia);

    return mount(AuthorizationView, {
      global: {
        plugins: [pinia],
        stubs: {
          RouterView: true,
        },
        provide: {
          [authorizationStoreSymbol as symbol]: authorizationStore,
        },
      },
    });
  };

  it('renders correctly', async () => {
    const wrapper = generateWrapper();

    await flushPromises();

    expect(wrapper.find('.container').exists()).toBe(true);
    expect(wrapper.find('.tabs').exists()).toBe(true);
    expect(wrapper.text()).toContain('Project');
    expect(wrapper.text()).toContain('AlwaysAllowed');
    expect(wrapper.find('.permission-card').exists()).toBe(true);
    expect(wrapper.find('.cards-grid').exists()).toBe(true);
  });

  it('calls fetchMe and fetchAll on mount', () => {
    generateWrapper();
    const tokenStore = useAuthorizationStore();
    expect(tokenStore.fetchResources).toHaveBeenCalled();
  });
});
