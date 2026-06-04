import { describe, expect, it } from 'vitest';
import { VueWrapper, flushPromises, mount } from '@vue/test-utils';
import { setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import {
  officeLocationRoutingSymbol,
  officeLocationStoreSymbol,
} from '@/store/injectionSymbols';
import { FormItem } from 'ant-design-vue';
import CreateOfficeLocationView from '../CreateOfficeLocationView.vue';
import type { OfficeLocationModel } from '@/models/OfficeLocation';
import { useOfficeLocationStore } from '@/store';
import { useFormStore } from '@/components/Form';

describe('CreateOfficeLocationView.vue', () => {
  setActivePinia(createTestingPinia({ stubActions: false }));

  let wrapper: VueWrapper;
  afterEach(() => {
    if (wrapper) {
      wrapper.unmount();
    }
  });

  const mockOfficeLocationRoutingService = {
    setOfficeLocationId: vi.fn((id: number) => Promise.resolve()),
  };

  it('renders correctly', () => {
    wrapper = mount(CreateOfficeLocationView, {
      global: {
        provide: {
          [officeLocationStoreSymbol as symbol]: useOfficeLocationStore(),
          [officeLocationRoutingSymbol as symbol]:
            mockOfficeLocationRoutingService,
        },
      },
    });

    const formItems = wrapper.findAllComponents(FormItem);
    expect(formItems).toHaveLength(1);

    expect(formItems[0].find('input').attributes('placeholder')).toBe(
      'Office Location Name',
    );
  });

  it('verifies a valid office location name correctly', async () => {
    const testData: OfficeLocationModel[] = [
      {
        id: 1,
        officeLocationName: 'Test Name',
      },
    ];

    wrapper = mount(CreateOfficeLocationView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            officeLocation: {
              officeLocations: testData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [officeLocationStoreSymbol as symbol]: useOfficeLocationStore(),
          [officeLocationRoutingSymbol as symbol]:
            mockOfficeLocationRoutingService,
        },
      },
    });

    const emailField = wrapper.findAllComponents(FormItem)[0];

    await emailField.find('.ant-input').setValue('test');
    await flushPromises();

    expect(
      emailField.find('.ant-form-item-feedback-icon-success').exists(),
    ).toBe(true);
  });

  it('verifies an invalid office location name correctly', async () => {
    const testData: OfficeLocationModel[] = [
      {
        id: 1,
        officeLocationName: 'Test Name',
      },
    ];

    wrapper = mount(CreateOfficeLocationView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            officeLocation: {
              officeLocations: testData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [officeLocationStoreSymbol as symbol]: useOfficeLocationStore(),
          [officeLocationRoutingSymbol as symbol]:
            mockOfficeLocationRoutingService,
        },
      },
    });

    const officeLocationNameField = wrapper.findAllComponents(FormItem)[0];

    await officeLocationNameField.find('.ant-input').setValue('Test Name');
    await flushPromises();

    expect(
      officeLocationNameField
        .find('.ant-form-item-feedback-icon-error')
        .exists(),
    ).toBe(true);
  });

  it('submits the form correctly', async () => {
    const officeLocationStore = useOfficeLocationStore();
    const formStore = useFormStore('CreateOfficeLocationForm');
    const createSpy = vi
      .spyOn(officeLocationStore, 'create')
      .mockImplementation(() => Promise.resolve(1));

    wrapper = mount(CreateOfficeLocationView, {
      global: {
        stubs: {
          contextHolder: true,
        },
        provide: {
          [officeLocationStoreSymbol as symbol]: officeLocationStore,
          [officeLocationRoutingSymbol as symbol]:
            mockOfficeLocationRoutingService,
        },
      },
    });

    const formInputs = wrapper.findAllComponents(FormItem);

    await formInputs[0].find('.ant-input').setValue('Test OfficeLocation');

    await flushPromises();

    await formStore.submit();
    await flushPromises();

    expect(createSpy).toHaveBeenCalled();
    expect(createSpy).toHaveBeenCalledWith({
      officeLocationName: 'Test OfficeLocation',
    });
  });
});
