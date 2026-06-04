import { flushPromises, mount } from '@vue/test-utils';
import { describe, expect, it, vi } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import {
  departmentRoutingSymbol,
  departmentStoreSymbol,
} from '@/store/injectionSymbols';
import { useDepartmentStore } from '@/store';
import { PlusOutlined } from '@ant-design/icons-vue';
import { DepartmentListView } from '..';

vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
  }),
}));

const departmentData1 = {
  id: '100',
  departmentName: 'Department1',
};
const departmentData2 = {
  id: '200',
  departmentName: 'Department2',
};

describe('DepartmentListView.vue', () => {
  const generateWrapper = () => {
    createTestingPinia({
      stubActions: true,
      initialState: {
        department: {
          departments: [departmentData1, departmentData2],
          isLoading: false,
        },
      },
    });

    const departmentStore = useDepartmentStore();

    const mockDepartmentRouting = {
      routerDepartmentId: ref(''),
      setDepartmentId: vi.fn(),
    };

    return mount(DepartmentListView, {
      global: {
        components: {
          PlusOutlined,
        },
        stubs: {
          RouterView: true,
        },
        provide: {
          [departmentStoreSymbol as symbol]: departmentStore,
          [departmentRoutingSymbol as symbol]: mockDepartmentRouting,
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

    expect(wrapper.text()).toContain('Create Department');

    expect(wrapper.text()).toContain('Department1');
    expect(wrapper.text()).toContain('Department2');
  });

  it('calls fetchAll on mount', () => {
    generateWrapper();
    const userStore = useDepartmentStore();
    expect(userStore.fetchAll).toHaveBeenCalled();
  });
});
