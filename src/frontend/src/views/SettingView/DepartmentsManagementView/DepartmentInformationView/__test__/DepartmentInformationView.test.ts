import { mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import {
  departmentRoutingSymbol,
  departmentStoreSymbol,
} from '@/store/injectionSymbols';
import { useDepartmentStore } from '@/store';
import router from '@/router';
import type { DepartmentModel } from '@/models/Department';

import { DepartmentInformationView } from '..';
import { useDepartmentRouting } from '@/utils/hooks/useDepartmentRouting';

const departmentData1: DepartmentModel = {
  id: 100,
  departmentName: 'Department1',

};

const mockRoute = {
  path: '/mock-path',
  query: { departmentId: '200' },
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

describe('DepartmentInformationView.vue', () => {
  setActivePinia(createPinia());
  const departmentStore = useDepartmentStore();

  const generateWrapper = () => {
    return mount(DepartmentInformationView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        provide: {
          [departmentStoreSymbol as symbol]: departmentStore,
          [departmentRoutingSymbol as symbol]: useDepartmentRouting(),
        },
        plugins: [router],
      },
    });
  };

  it('renders correctly', () => {
    mockRoute.query.departmentId = '100';
    departmentStore.setDepartment(departmentData1);
    const wrapper = generateWrapper();
    expect(wrapper.find('.label').exists()).toBe(true);
    const text = wrapper.findAll('.text');
    expect(text[0].text()).toBe(departmentData1.departmentName);
  });

});
