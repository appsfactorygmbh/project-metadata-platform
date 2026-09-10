import { describe, expect, it } from 'vitest';
import { VueWrapper, flushPromises, mount } from '@vue/test-utils';
import { setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import {
  globalBillingRoutingSymbol,
  globalBillingStoreSymbol,
} from '@/store/injectionSymbols';
import { FormItem } from 'ant-design-vue';
import CreateGlobalBillingView from '../CreateGlobalBillingView.vue';
import type { GlobalBillingModel } from '@/models/GlobalBilling';
import { useGlobalBillingStore } from '@/store';
import { useFormStore } from '@/components/Form';
import { ResourceActions } from '@/models/utils/ResourceActions.ts';

describe('CreateGlobalBillingView.vue', () => {
  setActivePinia(createTestingPinia({ stubActions: false }));

  let wrapper: VueWrapper;
  afterEach(() => {
    if (wrapper) {
      wrapper.unmount();
    }
  });

  const mockGlobalBillingRoutingService = {
    setGlobalBillingId: vi.fn((id: number) => Promise.resolve()),
  };
  const globalBillingStore = useGlobalBillingStore();
  // @ts-expect-error: Overriding getter for testing purposes
  globalBillingStore.getPermissions = [ResourceActions.Create];

  it('renders correctly', () => {
    wrapper = mount(CreateGlobalBillingView, {
      global: {
        provide: {
          [globalBillingStoreSymbol as symbol]: globalBillingStore,
          [globalBillingRoutingSymbol as symbol]:
            mockGlobalBillingRoutingService,
        },
      },
    });

    const formItems = wrapper.findAllComponents(FormItem);
    expect(formItems).toHaveLength(4);

    expect(formItems[0].find('input').attributes('placeholder')).toBe(
      'GlobalBilling Kind',
    );
    expect(formItems[2].find('input').attributes('placeholder')).toBe(
      'Target Margin',
    );
  });

  it('verifies a valid globalBilling kind correctly', async () => {
    const testData: GlobalBillingModel[] = [
      {
        id: 1,
        billingKind: 'Test Kind',
      },
    ];

    wrapper = mount(CreateGlobalBillingView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            globalBilling: {
              billingList: testData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [globalBillingStoreSymbol as symbol]: globalBillingStore,
          [globalBillingRoutingSymbol as symbol]:
            mockGlobalBillingRoutingService,
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

  it('verifies an invalid globalBilling kind correctly', async () => {
    const testData: GlobalBillingModel[] = [
      {
        id: 1,
        billingKind: 'Test Kind',
      },
    ];
    globalBillingStore.$patch({ billingList: testData });
    wrapper = mount(CreateGlobalBillingView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            globalBilling: {
              billingList: testData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [globalBillingStoreSymbol as symbol]: globalBillingStore,
          [globalBillingRoutingSymbol as symbol]:
            mockGlobalBillingRoutingService,
        },
      },
    });

    const globalBillingKindField = wrapper.findAllComponents(FormItem)[0];

    await globalBillingKindField.find('.ant-input').setValue('Test Kind');
    await flushPromises();

    expect(
      globalBillingKindField
        .find('.ant-form-item-feedback-icon-error')
        .exists(),
    ).toBe(true);
  });

  it('submits the form correctly', async () => {
    const formStore = useFormStore('CreateGlobalBillingForm');
    const createSpy = vi
      .spyOn(globalBillingStore, 'create')
      .mockImplementation(() => Promise.resolve(1));

    wrapper = mount(CreateGlobalBillingView, {
      global: {
        stubs: {
          contextHolder: true,
        },
        provide: {
          [globalBillingStoreSymbol as symbol]: globalBillingStore,
          [globalBillingRoutingSymbol as symbol]:
            mockGlobalBillingRoutingService,
        },
      },
    });

    const formInputs = wrapper.findAllComponents(FormItem);

    await formInputs[0].find('.ant-input').setValue('Test GlobalBilling');

    await flushPromises();

    await formStore.submit();
    await flushPromises();

    expect(createSpy).toHaveBeenCalled();
    expect(createSpy).toHaveBeenCalledWith({
      currency: undefined,
      billingKind: 'Test GlobalBilling',
      targetMargin: undefined,
      timeFrame: undefined,
    });
  });
});
