import { flushPromises, mount } from '@vue/test-utils';
import { describe, expect, it, vi } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import {
  apiTokenRoutingSymbol,
  apiTokenStoreSymbol,
} from '@/store/injectionSymbols';
import { useApiTokenStore } from '@/store';
import { PlusOutlined } from '@ant-design/icons-vue';
import { ApiTokenListView } from '..';

vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
  }),
}));

const tokenData1 = {
  id: 100,
  name: 'Token1',
  scopes: ['SCIM'],
  expirationDate: new Date(),
};
const tokenData2 = {
  id: 200,
  name: 'Token2',
  scopes: ['SCIM'],
  expirationDate: new Date(),
};

describe('ApiTokenListView.vue', () => {
  const generateWrapper = () => {
    const pinia = createTestingPinia({
      stubActions: true,
      initialState: {
        apiToken: { apiTokens: [tokenData1, tokenData2], isLoading: false },
      },
    });

    const tokenStore = useApiTokenStore();

    const mockApiTokenRouting = {
      routerApiTokenId: ref(''),
      setApiTokenId: vi.fn(),
    };

    return mount(ApiTokenListView, {
      global: {
        plugins: [pinia],
        components: {
          PlusOutlined,
        },
        stubs: {
          RouterView: true,
        },
        provide: {
          [apiTokenStoreSymbol as symbol]: tokenStore,
          [apiTokenRoutingSymbol as symbol]: mockApiTokenRouting,
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

    expect(wrapper.text()).toContain('Create New API-Token');

    expect(wrapper.text()).toContain('Token1');
    expect(wrapper.text()).toContain('Token2');
  });

  it('calls fetchMe and fetchAll on mount', () => {
    generateWrapper();
    const tokenStore = useApiTokenStore();
    expect(tokenStore.fetchAll).toHaveBeenCalled();
  });
});
