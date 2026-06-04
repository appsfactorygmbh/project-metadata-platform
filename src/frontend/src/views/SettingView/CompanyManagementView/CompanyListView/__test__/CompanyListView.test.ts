import { flushPromises, mount } from '@vue/test-utils';
import { describe, expect, it, vi } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import { companyRoutingSymbol, companyStoreSymbol } from '@/store/injectionSymbols';
import { useCompanyStore } from '@/store';
import { PlusOutlined } from '@ant-design/icons-vue';
import { CompanyListView } from '..';

vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
  }),
}));

const companyData1 = {
  id: '100',
  companyName: 'Company1',

};
const companyData2 = {
  id: '200',
  companyName: 'Company2',

};

describe('CompanyListView.vue', () => {
  const generateWrapper = () => {
    createTestingPinia({
      stubActions: true,
      initialState: {
        company: { companies: [companyData1, companyData2], isLoading: false },
      },
    });

    const companyStore = useCompanyStore();

    const mockCompanyRouting = {
      routerCompanyId: ref(''),
      setCompanyId: vi.fn(),
    };

    return mount(CompanyListView, {
      global: {
        components: {
          PlusOutlined,
        },
        stubs: {
          RouterView: true,
        },
        provide: {
          [companyStoreSymbol as symbol]: companyStore,
          [companyRoutingSymbol as symbol]: mockCompanyRouting,
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

    expect(wrapper.text()).toContain('Create Company');

    expect(wrapper.text()).toContain('Company1');
    expect(wrapper.text()).toContain('Company2');
  });

  it('calls fetchAll on mount', () => {
    generateWrapper();
    const userStore = useCompanyStore();
    expect(userStore.fetchAll).toHaveBeenCalled();
  });
});
