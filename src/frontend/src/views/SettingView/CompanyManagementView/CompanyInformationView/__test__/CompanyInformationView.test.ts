import { mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import {
  companyRoutingSymbol,
  companyStoreSymbol,
} from '@/store/injectionSymbols';
import { useCompanyStore } from '@/store';
import router from '@/router';
import type { CompanyModel } from '@/models/Company';

import { CompanyInformationView } from '..';
import { useCompanyRouting } from '@/utils/hooks/useCompanyRouting';

const companyData1: CompanyModel = {
  id: 100,
  companyName: 'Company1',
};

const mockRoute = {
  path: '/mock-path',
  query: { companyId: '200' },
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

describe('CompanyInformationView.vue', () => {
  setActivePinia(createPinia());
  const companyStore = useCompanyStore();

  const generateWrapper = () => {
    return mount(CompanyInformationView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        provide: {
          [companyStoreSymbol as symbol]: companyStore,
          [companyRoutingSymbol as symbol]: useCompanyRouting(),
        },
        plugins: [router],
      },
    });
  };

  it('renders correctly', () => {
    mockRoute.query.companyId = '100';
    companyStore.setCompany(companyData1);
    const wrapper = generateWrapper();
    expect(wrapper.find('.label').exists()).toBe(true);
    const text = wrapper.findAll('.text');
    expect(text[0].text()).toBe(companyData1.companyName);
  });
});
