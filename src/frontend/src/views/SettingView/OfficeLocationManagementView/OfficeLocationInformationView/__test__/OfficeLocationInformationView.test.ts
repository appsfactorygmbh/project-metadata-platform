import { mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import {
  officeLocationRoutingSymbol,
  officeLocationStoreSymbol,
} from '@/store/injectionSymbols';
import { useOfficeLocationStore } from '@/store';
import router from '@/router';
import type { OfficeLocationModel } from '@/models/OfficeLocation';

import { OfficeLocationInformationView } from '..';
import { useOfficeLocationRouting } from '@/utils/hooks/useOfficeLocationRouting';

const officeLocationData1: OfficeLocationModel = {
  id: 100,
  officeLocationName: 'OfficeLocation1',

};

const mockRoute = {
  path: '/mock-path',
  query: { officeLocationId: '200' },
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

describe('OfficeLocationInformationView.vue', () => {
  setActivePinia(createPinia());
  const officeLocationStore = useOfficeLocationStore();

  const generateWrapper = () => {
    return mount(OfficeLocationInformationView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        provide: {
          [officeLocationStoreSymbol as symbol]: officeLocationStore,
          [officeLocationRoutingSymbol as symbol]: useOfficeLocationRouting(),
        },
        plugins: [router],
      },
    });
  };

  it('renders correctly', () => {
    mockRoute.query.officeLocationId = '100';
    officeLocationStore.setOfficeLocation(officeLocationData1);
    const wrapper = generateWrapper();
    expect(wrapper.find('.label').exists()).toBe(true);
    const text = wrapper.findAll('.text');
    expect(text[0].text()).toBe(officeLocationData1.officeLocationName);
  });

});
