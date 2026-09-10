import { flushPromises, mount } from '@vue/test-utils';
import { describe, expect, it, vi } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import {
  globalBillingRoutingSymbol,
  globalBillingStoreSymbol,
} from '@/store/injectionSymbols';
import { useGlobalBillingStore } from '@/store';
import { PlusOutlined } from '@ant-design/icons-vue';
import { GlobalBillingListView } from '..';
import { ResourceActions } from '@/models/utils';

vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
  }),
}));

const globalBillingData1 = {
  id: '100',
  billingKind: 'GlobalBilling1',
};
const globalBillingData2 = {
  id: '200',
  billingKind: 'GlobalBilling2',
};

describe('GlobalBillingListView.vue', () => {
  const generateWrapper = () => {
    createTestingPinia({
      stubActions: true,
      initialState: {
        globalBilling: {
          billingList: [globalBillingData1, globalBillingData2],
          isLoading: false,
        },
      },
    });

    const globalBillingStore = useGlobalBillingStore();
    // @ts-expect-error: Overriding getter for testing purposes
    globalBillingStore.getPermissions = [ResourceActions.Create];

    const mockGlobalBillingRouting = {
      routerGlobalBillingId: ref(''),
      setGlobalBillingId: vi.fn(),
    };

    return mount(GlobalBillingListView, {
      global: {
        components: {
          PlusOutlined,
        },
        stubs: {
          RouterView: true,
        },
        provide: {
          [globalBillingStoreSymbol as symbol]: globalBillingStore,
          [globalBillingRoutingSymbol as symbol]: mockGlobalBillingRouting,
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

    expect(wrapper.text()).toContain('Create GlobalBilling');

    expect(wrapper.text()).toContain('GlobalBilling1');
    expect(wrapper.text()).toContain('GlobalBilling2');
  });

  it('calls fetchAll on mount', () => {
    generateWrapper();
    const userStore = useGlobalBillingStore();
    expect(userStore.fetchAll).toHaveBeenCalled();
  });
});
