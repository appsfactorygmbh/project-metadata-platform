import { flushPromises, mount } from '@vue/test-utils';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { type TestingPinia, createTestingPinia } from '@pinia/testing';
import BillingComponent from '../BillingComponent.vue';
import { useBillingStore } from '@/store';
import { ResourceActions } from '@/models/utils';
import {
  Currencies,
  type GetPluginBillingResponse,
  TimeFrame,
} from '@/api/generated';
import router from '@/router/router.ts';

describe('BillingComponent.vue', () => {
  let testingPinia: TestingPinia;

  beforeEach(() => {
    vi.clearAllMocks();

    testingPinia = createTestingPinia({
      createSpy: vi.fn,
      stubActions: false,
    });
  });

  const generateWrapper = (propsOverrides = {}) => {
    const billingStore = useBillingStore();

    vi.spyOn(billingStore, 'getBilling', 'get').mockReturnValue({
      projectId: 1,
      pluginId: 100,
      contractIds: ['C-123', 'C-456'],
      currency: Currencies.Eur,
      budgetLimit: 5000,
      hostingFee: 100,
      targetMargin: 15,
      timeFrame: TimeFrame.Monthly,
      date: undefined,
      notes: 'Test billing notes',
      permissions: [ResourceActions.Edit, ResourceActions.Delete],
    } as GetPluginBillingResponse);

    return mount(BillingComponent, {
      props: {
        projectId: 1,
        pluginId: 100,
        isArchived: false,
        ...propsOverrides,
      },
      global: {
        plugins: [router, testingPinia],
        stubs: {
          ConfirmationDialog: true,
          'a-popover': {
            template:
              '<div><slot name="title" /><slot name="content" /><slot /></div>',
          },
        },
      },
    });
  };

  it('renders the billing trigger badge correctly', () => {
    const wrapper = generateWrapper();
    const badge = wrapper.findComponent({ name: 'CreditCardOutlined' });
    expect(badge.exists()).toBe(true);
  });

  it('fetches billing data when the badge is clicked', async () => {
    const wrapper = generateWrapper();
    const billingStore = useBillingStore();

    const badge = wrapper.findComponent({ name: 'CreditCardOutlined' });
    await badge.trigger('click');

    expect(billingStore.fetch).toHaveBeenCalledWith(1, 100);
  });

  it('displays billing information correctly in the popover', async () => {
    const wrapper = generateWrapper();

    await wrapper
      .findComponent({ name: 'CreditCardOutlined' })
      .trigger('click');
    await flushPromises();

    const text = wrapper.text();

    expect(text).toContain('Billing Information');
    expect(text).toContain('C-123, C-456');
    expect(text).toContain('Test billing notes');
  });

  it('toggles edit mode when the Edit button is clicked', async () => {
    const wrapper = generateWrapper();

    await wrapper
      .findComponent({ name: 'CreditCardOutlined' })
      .trigger('click');
    await flushPromises();

    const buttons = wrapper.findAllComponents({ name: 'AButton' });
    const editButton = buttons.find((b) => b.text().includes('Edit'));

    expect(editButton).toBeDefined();
    await editButton!.trigger('click');
    await flushPromises();

    const updatedButtons = wrapper.findAllComponents({ name: 'AButton' });
    expect(updatedButtons.some((b) => b.text().includes('Cancel'))).toBe(true);
    expect(updatedButtons.some((b) => b.text().includes('Save'))).toBe(true);

    expect(updatedButtons.some((b) => b.text().includes('Edit'))).toBe(false);
    expect(updatedButtons.some((b) => b.text().includes('Delete'))).toBe(false);
  });

  it('opens the confirmation modal when Delete is clicked', async () => {
    const wrapper = generateWrapper();

    await wrapper
      .findComponent({ name: 'CreditCardOutlined' })
      .trigger('click');
    await flushPromises();

    const buttons = wrapper.findAllComponents({ name: 'AButton' });
    const deleteButton = buttons.find((b) => b.text().includes('Delete'));

    await deleteButton!.trigger('click');
    await flushPromises();

    const confirmModal = wrapper.findComponent({ name: 'ConfirmationDialog' });
    expect(confirmModal.exists()).toBe(true);
    expect(confirmModal.props('isOpen')).toBe(true);
  });

  it('hides edit and delete buttons if the project is archived', async () => {
    const wrapper = generateWrapper({ isArchived: true });

    await wrapper
      .findComponent({ name: 'CreditCardOutlined' })
      .trigger('click');
    await flushPromises();

    const buttons = wrapper.findAllComponents({ name: 'AButton' });
    expect(buttons.some((b) => b.text().includes('Edit'))).toBe(false);
    expect(buttons.some((b) => b.text().includes('Delete'))).toBe(false);
  });
});
