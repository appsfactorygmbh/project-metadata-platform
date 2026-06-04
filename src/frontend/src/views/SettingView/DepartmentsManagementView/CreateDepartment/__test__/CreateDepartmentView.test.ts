import { describe, expect, it } from 'vitest';
import { VueWrapper, flushPromises, mount } from '@vue/test-utils';
import { setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import {
  departmentRoutingSymbol,
  departmentStoreSymbol,
} from '@/store/injectionSymbols';
import { FormItem } from 'ant-design-vue';
import CreateDepartmentView from '../CreateDepartmentView.vue';
import type { DepartmentModel } from '@/models/Department';
import { useDepartmentStore } from '@/store';
import { useFormStore } from '@/components/Form';

describe('CreateDepartmentView.vue', () => {
  setActivePinia(createTestingPinia({ stubActions: false }));

  let wrapper: VueWrapper;
  afterEach(() => {
    if (wrapper) {
      wrapper.unmount();
    }
  });

  const mockDepartmentRoutingService = {
    setDepartmentId: vi.fn((id: number) => Promise.resolve()),
  };

  it('renders correctly', () => {
    wrapper = mount(CreateDepartmentView, {
      global: {
        provide: {
          [departmentStoreSymbol as symbol]: useDepartmentStore(),
          [departmentRoutingSymbol as symbol]: mockDepartmentRoutingService,
        },
      },
    });

    const formItems = wrapper.findAllComponents(FormItem);
    expect(formItems).toHaveLength(1);

    expect(formItems[0].find('input').attributes('placeholder')).toBe(
      'Department Name',
    );
  });

  it('verifies a valid department name correctly', async () => {
    const testData: DepartmentModel[] = [
      {
        id: 1,
        departmentName: 'Test Name',
      },
    ];

    wrapper = mount(CreateDepartmentView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            department: {
              departments: testData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [departmentStoreSymbol as symbol]: useDepartmentStore(),
          [departmentRoutingSymbol as symbol]: mockDepartmentRoutingService,
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

  it('verifies an invalid department name correctly', async () => {
    const testData: DepartmentModel[] = [
      {
        id: 1,
        departmentName: 'Test Name',
      },
    ];

    wrapper = mount(CreateDepartmentView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            department: {
              departments: testData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [departmentStoreSymbol as symbol]: useDepartmentStore(),
          [departmentRoutingSymbol as symbol]: mockDepartmentRoutingService,
        },
      },
    });

    const departmentNameField = wrapper.findAllComponents(FormItem)[0];

    await departmentNameField.find('.ant-input').setValue('Test Name');
    await flushPromises();

    expect(
      departmentNameField.find('.ant-form-item-feedback-icon-error').exists(),
    ).toBe(true);
  });

  it('submits the form correctly', async () => {
    const departmentStore = useDepartmentStore();
    const formStore = useFormStore('CreateDepartmentForm');
    const createSpy = vi
      .spyOn(departmentStore, 'create')
      .mockImplementation(() => Promise.resolve(1));

    wrapper = mount(CreateDepartmentView, {
      global: {
        stubs: {
          contextHolder: true,
        },
        provide: {
          [departmentStoreSymbol as symbol]: departmentStore,
          [departmentRoutingSymbol as symbol]: mockDepartmentRoutingService,
        },
      },
    });

    const formInputs = wrapper.findAllComponents(FormItem);

    await formInputs[0].find('.ant-input').setValue('Test Department');

    await flushPromises();

    await formStore.submit();
    await flushPromises();

    expect(createSpy).toHaveBeenCalled();
    expect(createSpy).toHaveBeenCalledWith({
      departmentName: 'Test Department',
    });
  });
});
