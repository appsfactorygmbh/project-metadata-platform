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
    const inputFields = ['E-Mail', 'Password', 'Confirm Password'];

    wrapper = mount(CreateUserView, {
      global: {
        provide: {
          [userStoreSymbol as symbol]: useUserStore(),
        },
      },
    });

    const formItems = wrapper.findAllComponents(FormItem);
    expect(formItems).toHaveLength(3);

    for (let i = 0; i < formItems.length; i++) {
      expect(formItems[i].find('input').attributes('placeholder')).toBe(
        inputFields[i],
      );
    }
  });

  it('verifies the email correctly', async () => {
    const testData: UserListModel[] = [
      {
        id: 1,
        email: 'a@b.cd',
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

    const emailField = wrapper.findAllComponents(FormItem)[0];

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

    const passwordField = wrapper.findAllComponents(FormItem)[1];
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

    const passwordField = wrapper.findAllComponents(FormItem)[1];
    const confirmPasswordField = wrapper.findAllComponents(FormItem)[2];

    await passwordField.find('.ant-input').setValue('a');
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
    const testData = ['E@Ma.il', 'Pa$$w0rd', 'Pa$$w0rd'];

    const userStore = useUserStore();
    const formStore = useFormStore('createUserForm');

    wrapper = mount(CreateUserView, {
      global: {
        provide: {
          [userStoreSymbol as symbol]: userStore,
        },
      },
    });

    const formInputs = wrapper.findAllComponents(FormItem);

    for (let i = 0; i < formInputs.length; i++) {
      await formInputs[i].find('.ant-input').setValue(testData[i]);
    }
    await flushPromises();

    await formStore.submit();
    await flushPromises();

    expect(userStore.create).toHaveBeenCalledWith({
      email: 'E@Ma.il',
      password: 'Pa$$w0rd',
    });
  });
});
