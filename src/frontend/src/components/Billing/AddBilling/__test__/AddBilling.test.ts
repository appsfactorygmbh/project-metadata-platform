import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import { type TestingPinia, createTestingPinia } from '@pinia/testing';
import { setActivePinia } from 'pinia';
import AddBillingForm from '../AddBillingForm.vue';
import {
  useBillingStore,
  useGlobalBillingStore,
  useLocalLogStore,
} from '@/store';
import { useFormStore } from '@/components/Form';
import { Currencies, TimeFrame } from '@/api/generated/index';
import type { GlobalBillingModel } from '@/models/GlobalBilling/GlobalBillingModel.ts';

interface TestBillingFormState {
  billingId?: number;
  contractIds: string[];
  currency?: Currencies;
  budgetLimit?: number;
  hostingFee?: number;
  targetMargin?: number;
  timeFrame?: TimeFrame;
  date?: Date;
  notes?: string;
  inputsDisabled: boolean;
}

describe('AddBillingForm.vue', () => {
  let testingPinia: TestingPinia;

  beforeEach(() => {
    vi.clearAllMocks();

    testingPinia = createTestingPinia({ stubActions: false });
    setActivePinia(testingPinia);
  });

  const generateWrapper = () => {
    const globalBillingStore = useGlobalBillingStore();
    const formStore = useFormStore('addBillingForm');

    vi.spyOn(globalBillingStore, 'getGlobalBillingList', 'get').mockReturnValue(
      [
        {
          id: 1,
          billingKind: 'Standard Template',
        },
      ],
    );

    vi.spyOn(globalBillingStore, 'getGlobalBilling', 'get').mockReturnValue({
      id: 1,
      currency: Currencies.Usd,
      targetMargin: 20,
      timeFrame: TimeFrame.Monthly,
    } as GlobalBillingModel);

    return mount(AddBillingForm, {
      props: {
        projectId: 1,
        pluginId: 100,
        formStore,
        initialValues: {
          billingId: undefined,
          contractIds: [],
          currency: undefined,
          budgetLimit: undefined,
          hostingFee: undefined,
          targetMargin: undefined,
          timeFrame: undefined,
          date: undefined,
          notes: undefined,
          inputsDisabled: true,
        },
      },
      global: {
        plugins: [testingPinia],
        stubs: {
          'a-tooltip': { template: '<div><slot /></div>' },
        },
      },
    });
  };

  it('renders correctly and populates the billing template dropdown', async () => {
    const wrapper = generateWrapper();
    await flushPromises();

    const formItems = wrapper.findAll('.ant-form-item');
    expect(formItems.length).toBeGreaterThanOrEqual(8);

    const globalBillingStore = useGlobalBillingStore();
    expect(globalBillingStore.fetchAll).toHaveBeenCalled();
  });

  it('autofills currency, margin, and timeframe when a template is selected', async () => {
    const wrapper = generateWrapper();
    await flushPromises();

    const globalBillingStore = useGlobalBillingStore();

    const billingSelect = wrapper.findAllComponents({ name: 'ASelect' })[0];
    await billingSelect.vm.$emit('change', 1);
    await flushPromises();

    expect(globalBillingStore.fetch).toHaveBeenCalledWith(1);

    const dynamicForm = (
      wrapper.vm as unknown as { dynamicValidateForm: TestBillingFormState }
    ).dynamicValidateForm;
    expect(dynamicForm.inputsDisabled).toBe(false);
    expect(dynamicForm.currency).toBe(Currencies.Usd);
    expect(dynamicForm.targetMargin).toBe(20);
    expect(dynamicForm.timeFrame).toBe(TimeFrame.Monthly);
  });

  it('submits the billing form correctly', async () => {
    const wrapper = generateWrapper();
    await flushPromises();

    const billingStore = useBillingStore();
    const logStore = useLocalLogStore();
    const formStore = useFormStore('addBillingForm');

    const addSpy = vi.spyOn(billingStore, 'add').mockResolvedValue([1, 100]);

    const dynamicForm = (
      wrapper.vm as unknown as { dynamicValidateForm: TestBillingFormState }
    ).dynamicValidateForm;
    dynamicForm.billingId = 1;
    dynamicForm.contractIds = ['C-123'];
    dynamicForm.budgetLimit = 5000;
    dynamicForm.hostingFee = 100;
    dynamicForm.currency = Currencies.Eur;
    dynamicForm.targetMargin = 15;
    dynamicForm.timeFrame = TimeFrame.Yearly;
    dynamicForm.notes = 'Test note';

    await formStore.submit();
    await flushPromises();

    expect(addSpy).toHaveBeenCalledWith(1, 100, {
      billingId: 1,
      contractIds: ['C-123'],
      currency: Currencies.Eur,
      budgetLimit: 5000,
      hostingFee: 100,
      targetMargin: 15,
      timeFrame: TimeFrame.Yearly,
      date: undefined,
      notes: 'Test note',
    });

    expect(logStore.fetch).toHaveBeenCalledWith(1);

    expect(wrapper.emitted()).toHaveProperty('addedBilling');
  });
});
