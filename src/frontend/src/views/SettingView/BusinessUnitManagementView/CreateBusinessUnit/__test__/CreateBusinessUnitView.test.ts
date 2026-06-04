import { describe, expect, it } from 'vitest';
import { VueWrapper, flushPromises, mount } from '@vue/test-utils';
import { setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import {
  businessUnitRoutingSymbol,
  businessUnitStoreSymbol,
} from '@/store/injectionSymbols';
import { FormItem } from 'ant-design-vue';
import CreateBusinessUnitView from '../CreateBusinessUnitView.vue';
import type { BusinessUnitModel } from '@/models/BusinessUnit';
import { useBusinessUnitStore } from '@/store';
import { useFormStore } from '@/components/Form';

describe('CreateBusinessUnitView.vue', () => {
  setActivePinia(createTestingPinia({ stubActions: false }));

  let wrapper: VueWrapper;
  afterEach(() => {
    if (wrapper) {
      wrapper.unmount();
    }
  });

  const mockBusinessUnitRoutingService = {
    setBusinessUnitId: vi.fn((id: number) => Promise.resolve()),
  };

  it('renders correctly', () => {
    wrapper = mount(CreateBusinessUnitView, {
      global: {
        provide: {
          [businessUnitStoreSymbol as symbol]: useBusinessUnitStore(),
          [businessUnitRoutingSymbol as symbol]: mockBusinessUnitRoutingService,
        },
      },
    });

    const formItems = wrapper.findAllComponents(FormItem);
    expect(formItems).toHaveLength(1);

    expect(formItems[0].find('input').attributes('placeholder')).toBe(
      'Business Unit Name',
    );
  });

  it('verifies a valid business unit name correctly', async () => {
    const testData: BusinessUnitModel[] = [
      {
        id: 1,
        businessUnitName: 'Test Name',
      },
    ];

    wrapper = mount(CreateBusinessUnitView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            businessUnit: {
              businessUnits: testData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [businessUnitStoreSymbol as symbol]: useBusinessUnitStore(),
          [businessUnitRoutingSymbol as symbol]: mockBusinessUnitRoutingService,
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

  it('verifies an invalid business unit name correctly', async () => {
    const testData: BusinessUnitModel[] = [
      {
        id: 1,
        businessUnitName: 'Test Name',
      },
    ];

    wrapper = mount(CreateBusinessUnitView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            businessUnit: {
              businessUnits: testData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [businessUnitStoreSymbol as symbol]: useBusinessUnitStore(),
          [businessUnitRoutingSymbol as symbol]: mockBusinessUnitRoutingService,
        },
      },
    });

    const businessUnitNameField = wrapper.findAllComponents(FormItem)[0];

    await businessUnitNameField.find('.ant-input').setValue('Test Name');
    await flushPromises();

    expect(
      businessUnitNameField.find('.ant-form-item-feedback-icon-error').exists(),
    ).toBe(true);
  });

  it('submits the form correctly', async () => {
    const businessUnitStore = useBusinessUnitStore();
    const formStore = useFormStore('CreateBusinessUnitForm');
    const createSpy = vi
      .spyOn(businessUnitStore, 'create')
      .mockImplementation(() => Promise.resolve(1));

    wrapper = mount(CreateBusinessUnitView, {
      global: {
        stubs: {
          contextHolder: true,
        },
        provide: {
          [businessUnitStoreSymbol as symbol]: businessUnitStore,
          [businessUnitRoutingSymbol as symbol]: mockBusinessUnitRoutingService,
        },
      },
    });

    const formInputs = wrapper.findAllComponents(FormItem);

    await formInputs[0].find('.ant-input').setValue('Test BusinessUnit');

    await flushPromises();

    await formStore.submit();
    await flushPromises();

    expect(createSpy).toHaveBeenCalled();
    expect(createSpy).toHaveBeenCalledWith({
      businessUnitName: 'Test BusinessUnit',
    });
  });
});
