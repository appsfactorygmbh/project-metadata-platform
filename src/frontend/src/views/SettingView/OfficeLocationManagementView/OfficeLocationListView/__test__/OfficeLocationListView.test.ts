import { flushPromises, mount } from '@vue/test-utils';
import { describe, expect, it, vi } from 'vitest';
import { createTestingPinia } from '@pinia/testing';
import {
  officeLocationRoutingSymbol,
  officeLocationStoreSymbol,
} from '@/store/injectionSymbols';
import { useOfficeLocationStore } from '@/store';
import { PlusOutlined } from '@ant-design/icons-vue';
import { OfficeLocationListView } from '..';

vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn(),
  }),
}));

const officeLocationData1 = {
  id: '100',
  officeLocationName: 'OfficeLocation1',
};
const officeLocationData2 = {
  id: '200',
  officeLocationName: 'OfficeLocation2',
};

describe('OfficeLocationListView.vue', () => {
  const generateWrapper = () => {
    createTestingPinia({
      stubActions: true,
      initialState: {
        officeLocation: {
          officeLocations: [officeLocationData1, officeLocationData2],
          isLoading: false,
        },
      },
    });

    const officeLocationStore = useOfficeLocationStore();

    const mockOfficeLocationRouting = {
      routerOfficeLocationId: ref(''),
      setOfficeLocationId: vi.fn(),
    };

    return mount(OfficeLocationListView, {
      global: {
        components: {
          PlusOutlined,
        },
        stubs: {
          RouterView: true,
        },
        provide: {
          [officeLocationStoreSymbol as symbol]: officeLocationStore,
          [officeLocationRoutingSymbol as symbol]: mockOfficeLocationRouting,
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

    expect(wrapper.text()).toContain('Create Office Location');

    expect(wrapper.text()).toContain('OfficeLocation1');
    expect(wrapper.text()).toContain('OfficeLocation2');
  });

  it('calls fetchAll on mount', () => {
    generateWrapper();
    const userStore = useOfficeLocationStore();
    expect(userStore.fetchAll).toHaveBeenCalled();
  });
});
