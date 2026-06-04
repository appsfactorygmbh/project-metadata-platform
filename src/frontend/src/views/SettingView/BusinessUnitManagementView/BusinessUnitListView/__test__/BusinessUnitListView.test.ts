import { flushPromises, mount } from '@vue/test-utils';
import { describe, expect, it, vi } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { businessUnitRoutingSymbol, businessUnitStoreSymbol } from '@/store/injectionSymbols';
import { useBusinessUnitStore } from '@/store';
import { PlusOutlined } from '@ant-design/icons-vue';
import { BusinessUnitListView } from '..';

vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
  }),
}));

const businessUnitData1 = {
  id: '100',
  businessUnitName: 'BusinessUnit1',

};
const businessUnitData2 = {
  id: '200',
  businessUnitName: 'BusinessUnit2',

};

describe('BusinessUnitListView.vue', () => {
  const generateWrapper = () => {
    createTestingPinia({
      stubActions: true,
      initialState: {
        businessUnit: { businessUnits: [businessUnitData1, businessUnitData2], isLoading: false },
      },
    });

    const businessUnitStore = useBusinessUnitStore();

    const mockBusinessUnitRouting = {
      routerBusinessUnitId: ref(''),
      setBusinessUnitId: vi.fn(),
    };

    return mount(BusinessUnitListView, {
      global: {
        components: {
          PlusOutlined,
        },
        stubs: {
          RouterView: true,
        },
        provide: {
          [businessUnitStoreSymbol as symbol]: businessUnitStore,
          [businessUnitRoutingSymbol as symbol]: mockBusinessUnitRouting,
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

    expect(wrapper.text()).toContain('Create Business Unit');

    expect(wrapper.text()).toContain('BusinessUnit1');
    expect(wrapper.text()).toContain('BusinessUnit2');
  });

  it('calls fetchAll on mount', () => {
    generateWrapper();
    const userStore = useBusinessUnitStore();
    expect(userStore.fetchAll).toHaveBeenCalled();
  });
});
