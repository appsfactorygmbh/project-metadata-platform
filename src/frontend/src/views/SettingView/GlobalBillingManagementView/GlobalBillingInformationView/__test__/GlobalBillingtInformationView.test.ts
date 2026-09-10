import { mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import {
  globalBillingRoutingSymbol,
  globalBillingStoreSymbol,
} from '@/store/injectionSymbols';
import { useGlobalBillingStore } from '@/store';
import router from '@/router';
import type { GlobalBillingModel } from '@/models/GlobalBilling';

import { GlobalBillingInformationView } from '..';
import { useGlobalBillingRouting } from '@/utils/hooks/useGlobalBillingRouting';
import { Currencies, TimeFrame } from '@/api/generated';

const globalBillingData1: GlobalBillingModel = {
  id: 100,
  billingKind: 'GlobalBilling1',
  currency: Currencies.Eur,
  targetMargin: 30,
  timeFrame: TimeFrame.Monthly,
};

const mockRoute = {
  path: '/mock-path',
  query: { globalBillingId: '200' },
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

describe('GlobalBillingInformationView.vue', () => {
  setActivePinia(createPinia());
  const globalBillingStore = useGlobalBillingStore();

  const generateWrapper = () => {
    return mount(GlobalBillingInformationView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        provide: {
          [globalBillingStoreSymbol as symbol]: globalBillingStore,
          [globalBillingRoutingSymbol as symbol]: useGlobalBillingRouting(),
        },
        plugins: [router],
      },
    });
  };

  it('renders correctly', () => {
    mockRoute.query.globalBillingId = '100';
    globalBillingStore.setGlobalBilling(globalBillingData1);
    const wrapper = generateWrapper();
    expect(wrapper.find('.label').exists()).toBe(true);
    const text = wrapper.findAll('.text');
    expect(text[0].text()).toBe(globalBillingData1.billingKind);
    expect(text[1].text()).toBe('Euro (EUR)');
    expect(text[2].text()).toBe('30%');
    expect(text[3].text()).toBe('Monthly');
  });
});
