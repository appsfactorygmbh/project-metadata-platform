import { describe, expect, it } from 'vitest';
import { CreateUserView } from '..';
import { VueWrapper, flushPromises, mount } from '@vue/test-utils';
import { setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import { userStoreSymbol } from '@/store/injectionSymbols';
import { useUserStore } from '@/store';
import { FormItem } from 'ant-design-vue';
import { useFormStore } from '@/components/Form';
import type { UserListModel } from '@/models/User';

describe('CreateUserView.vue', () => {
  setActivePinia(createTestingPinia({ stubActions: false }));

  let wrapper: VueWrapper;
  afterEach(() => {
    if (wrapper) {
      wrapper.unmount();
    }
  });

  it('renders correctly', () => {
    wrapper = mount(CreateUserView, {
      global: {
        provide: {
          [userStoreSymbol as symbol]: useUserStore(),
        },
      },
    });

    const formItems = wrapper.findAllComponents(FormItem);

    const getFormItem = (name: string) => {
      const item = formItems.find((c) => c.props('name') === name);
      if (!item) throw new Error(`FormItem with name '${name}' not found.`);
      return item;
    };
    const expectedInputs = [
      { name: 'employeeNumber', placeholder: 'Employee Number' },
      { name: 'email', placeholder: 'E-Mail' },
      { name: 'password', placeholder: 'Password' },
      { name: 'company', placeholder: 'Company' },
    ];

    expectedInputs.forEach(({ name, placeholder }) => {
      const itemWrapper = getFormItem(name);
      expect(itemWrapper.find('input').attributes('placeholder')).toBe(
        placeholder,
      );
    });

    const expectedSelects = [
      { name: 'jobTitles', placeholder: 'Jobtitles' },
      { name: 'teams', placeholder: 'Teams' },
      { name: 'teamSupport', placeholder: 'TeamSupport' },
      { name: 'departments', placeholder: 'Departments' },
      { name: 'businessUnits', placeholder: 'Business Units' },
    ];
    expectedSelects.forEach(({ name, placeholder }) => {
      const itemWrapper = getFormItem(name);

      const selectComponent = itemWrapper.findComponent(
        '.ant-select',
      ) as VueWrapper<any>;

      expect(selectComponent.exists()).toBe(true);
      expect(selectComponent.props('placeholder')).toBe(placeholder);
    });

    const confirmPasswordItem = formItems.find(
      (c) => c.props('name') === 'confirmPassword',
    );
    expect(confirmPasswordItem).toBeUndefined();
  });

  it('verifies the email correctly', async () => {
    const testData: UserListModel[] = [
      {
        externalId: '1',
        userName: 'a@b.cd',
        isScimProvisioned: true,
      },
    ];

    wrapper = mount(CreateUserView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            user: {
              users: testData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [userStoreSymbol as symbol]: useUserStore(),
        },
      },
    });

    const emailField = wrapper.findAllComponents(FormItem)[1];

    await emailField.find('.ant-input').setValue('a@bc.de');
    await flushPromises();

    expect(
      emailField.find('.ant-form-item-feedback-icon-success').exists(),
    ).toBe(true);

    await emailField.find('.ant-input').setValue('a');
    await flushPromises();

    expect(emailField.find('.ant-form-item-feedback-icon-error').exists()).toBe(
      true,
    );

    await emailField.find('.ant-input').setValue('a@b.cd');
    await flushPromises();

    expect(emailField.find('.ant-form-item-feedback-icon-error').exists()).toBe(
      true,
    );
  });

  it('verifies the password correctly', async () => {
    wrapper = mount(CreateUserView, {
      global: {
        provide: {
          [userStoreSymbol as symbol]: useUserStore(),
        },
      },
    });

    const passwordField = wrapper.findAllComponents(FormItem)[3];
    const passwordInput = passwordField.find('.ant-input');

    // Test a valid password
    await passwordField.find('.ant-input').setValue('aA6&&&&&');
    await flushPromises();
    expect(
      passwordField.find('.ant-form-item-feedback-icon-success').exists(),
    ).toBe(true);

    // Password must have lower case letters
    await passwordField.find('.ant-input').setValue('AAAAAA6&');
    await flushPromises();
    expect(
      passwordField.find('.ant-form-item-feedback-icon-error').exists(),
    ).toBe(true);

    // Password must have upper case letters
    await passwordInput.setValue('aaaaaa6&');
    await flushPromises();
    expect(
      passwordField.find('.ant-form-item-feedback-icon-error').exists(),
    ).toBe(true);

    // Password must have special characters
    await passwordInput.setValue('6AAAAAAa');
    await flushPromises();
    expect(
      passwordField.find('.ant-form-item-feedback-icon-error').exists(),
    ).toBe(true);

    // Password must have numbers
    await passwordInput.setValue('AAAAAAa&');
    await flushPromises();
    expect(
      passwordField.find('.ant-form-item-feedback-icon-error').exists(),
    ).toBe(true);

    // Password must have at least 8 characters
    await passwordInput.setValue('aA6&');
    await flushPromises();
    expect(
      passwordField.find('.ant-form-item-feedback-icon-error').exists(),
    ).toBe(true);
  });

  it('verifies the password confirm correctly', async () => {
    wrapper = mount(CreateUserView, {
      global: {
        provide: {
          [userStoreSymbol as symbol]: useUserStore(),
        },
      },
    });

    const passwordField = wrapper.findAllComponents(FormItem)[3];
    await passwordField.find('.ant-input').setValue('a');
    const confirmPasswordField = wrapper.findAllComponents(FormItem)[4];

    await confirmPasswordField.find('.ant-input').setValue('a');
    await flushPromises();

    expect(
      confirmPasswordField
        .find('.ant-form-item-feedback-icon-success')
        .exists(),
    ).toBe(true);

    await confirmPasswordField.find('.ant-input').setValue('b');
    await flushPromises();

    expect(
      confirmPasswordField.find('.ant-form-item-feedback-icon-error').exists(),
    ).toBe(true);
  });

  it('submits the form correctly', async () => {
    const userStore = useUserStore();
    const formStore = useFormStore('createUserForm');

    const createSpy = vi
      .spyOn(userStore, 'create')
      .mockImplementation(() => Promise.resolve());

    wrapper = mount(CreateUserView, {
      global: {
        provide: {
          [userStoreSymbol as symbol]: userStore,
        },
      },
    });

    await flushPromises();

    const getFormItem = (name: string) =>
      wrapper.findAllComponents(FormItem).find((c) => c.props('name') === name);

    await getFormItem('employeeNumber')!.find('.ant-input').setValue('1');
    await getFormItem('email')!.find('.ant-input').setValue('E@Ma.il');
    await getFormItem('password')!.find('.ant-input').setValue('Pa$$w0rd');

    await flushPromises();

    await getFormItem('confirmPassword')!
      .find('.ant-input')
      .setValue('Pa$$w0rd');
    await flushPromises();

    await formStore.submit();
    await flushPromises();

    expect(createSpy).toHaveBeenCalledWith(
      expect.objectContaining({
        externalId: '1',
        userName: 'E@Ma.il',
        password: 'Pa$$w0rd',
      }),
    );
  });
});
