import { mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import {
  businessUnitRoutingSymbol,
  businessUnitStoreSymbol,
} from '@/store/injectionSymbols';
import { useBusinessUnitStore } from '@/store';
import router from '@/router';
import type { BusinessUnitModel } from '@/models/BusinessUnit';

import { BusinessUnitInformationView } from '..';
import { useBusinessUnitRouting } from '@/utils/hooks';


const businessUnitData1: BusinessUnitModel = {
  id: 100,
  businessUnitName: 'BusinessUnit1',

};

const mockRoute = {
  path: '/mock-path',
  query: { businessUnitId: '200' },
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

describe('BusinessUnitInformationView.vue', () => {
  setActivePinia(createPinia());
  const businessUnitStore = useBusinessUnitStore();

  const generateWrapper = () => {
    return mount(BusinessUnitInformationView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        provide: {
          [businessUnitStoreSymbol as symbol]: businessUnitStore,
          [businessUnitRoutingSymbol as symbol]: useBusinessUnitRouting(),
        },
        plugins: [router],
      },
    });
  };

  it('renders correctly', () => {
    mockRoute.query.businessUnitId = '100';
    businessUnitStore.setBusinessUnit(businessUnitData1);
    const wrapper = generateWrapper();
    expect(wrapper.find('.label').exists()).toBe(true);
    const text = wrapper.findAll('.text');
    expect(text[0].text()).toBe(businessUnitData1.businessUnitName);
  });

});
