import { describe, expect, it } from 'vitest';
import { VueWrapper, flushPromises, mount } from '@vue/test-utils';
import { setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import { companyRoutingSymbol, companyStoreSymbol } from '@/store/injectionSymbols';
import { FormItem } from 'ant-design-vue';
import CreateCompanyView from '../CreateCompanyView.vue';
import type { CompanyModel } from '@/models/Company';
import { useCompanyStore } from '@/store';
import { useFormStore } from '@/components/Form';

describe('CreateCompanyView.vue', () => {
  setActivePinia(createTestingPinia({ stubActions: false }));

  let wrapper: VueWrapper;
  afterEach(() => {
    if (wrapper) {
      wrapper.unmount();
    }
  });

  const mockCompanyRoutingService = {
    setCompanyId: vi.fn((id: number) => Promise.resolve()),
  };

  it('renders correctly', () => {
    wrapper = mount(CreateCompanyView, {
      global: {
        provide: {
          [companyStoreSymbol as symbol]: useCompanyStore(),
          [companyRoutingSymbol as symbol]: mockCompanyRoutingService,
        },
      },
    });

    const formItems = wrapper.findAllComponents(FormItem);
    expect(formItems).toHaveLength(1);


    expect(formItems[0].find('input').attributes('placeholder')).toBe('Company Name');

  });

  it('verifies a valid company name correctly', async () => {
    const testData: CompanyModel[] = [
      {
        id: 1,
        companyName: 'Test Name',
      },
    ];

    wrapper = mount(CreateCompanyView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            company: {
              companies: testData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [companyStoreSymbol as symbol]: useCompanyStore(),
          [companyRoutingSymbol as symbol]: mockCompanyRoutingService,
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

  it('verifies an invalid company name correctly', async () => {
    const testData: CompanyModel[] = [
      {
        id: 1,
        companyName: 'Test Name',
      },
    ];

    wrapper = mount(CreateCompanyView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            company: {
              companies: testData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [companyStoreSymbol as symbol]: useCompanyStore(),
          [companyRoutingSymbol as symbol]: mockCompanyRoutingService,
        },
      },
    });

    const companyNameField = wrapper.findAllComponents(FormItem)[0];

    await companyNameField.find('.ant-input').setValue('Test Name');
    await flushPromises();

    expect(
      companyNameField.find('.ant-form-item-feedback-icon-error').exists(),
    ).toBe(true);
  });

  it('submits the form correctly', async () => {
    const companyStore = useCompanyStore();
    const formStore = useFormStore('CreateCompanyForm');
    const createSpy = vi
      .spyOn(companyStore, 'create')
      .mockImplementation(() => Promise.resolve(1));

    wrapper = mount(CreateCompanyView, {
      global: {
        stubs: {
          contextHolder: true,
        },
        provide: {
          [companyStoreSymbol as symbol]: companyStore,
          [companyRoutingSymbol as symbol]: mockCompanyRoutingService,
        },
      },
    });

    const formInputs = wrapper.findAllComponents(FormItem);

    await formInputs[0].find('.ant-input').setValue('Test Company');

    await flushPromises();

    await formStore.submit();
    await flushPromises();

    expect(createSpy).toHaveBeenCalled();
    expect(createSpy).toHaveBeenCalledWith({
      companyName: 'Test Company',

    });
  });
});
