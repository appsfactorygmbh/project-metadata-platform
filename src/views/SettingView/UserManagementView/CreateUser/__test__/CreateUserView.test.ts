import { describe, expect, it } from 'vitest';
import { CreateUserView } from '..';
import { flushPromises, mount } from '@vue/test-utils';
import { setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import { userStoreSymbol } from '@/store/injectionSymbols';
import { useUserStore } from '@/store';
import { FormItem } from 'ant-design-vue';
import { useFormStore } from '@/components/Form';

describe('CreateUserView.vue', () => {
  setActivePinia(createTestingPinia({ stubActions: false }));

  it('renders correctly', () => {
    const inputFields = [
      'Name',
      'Username',
      'E-Mail',
      'Password',
      'Confirm Password',
    ];

    const wrapper = mount(CreateUserView, {
      global: {
        provide: {
          [userStoreSymbol as symbol]: useUserStore(),
        },
      },
    });

    const formItems = wrapper.findAllComponents(FormItem);
    expect(formItems).toHaveLength(5);

    for (let i = 0; i < formItems.length; i++) {
      expect(formItems[i].find('input').attributes('placeholder')).toBe(
        inputFields[i],
      );
    }
  });

  it('verifies the email correctly', async () => {
    const wrapper = mount(CreateUserView, {
      global: {
        provide: {
          [userStoreSymbol as symbol]: useUserStore(),
        },
      },
    });

    const emailField = wrapper.findAllComponents(FormItem)[2];

    await emailField.find('.ant-input').setValue('a');
    await flushPromises();

    expect(emailField.find('.ant-form-item-feedback-icon-error').exists()).toBe(
      true,
    );

    await emailField.find('.ant-input').setValue('a@b.cd');
    await flushPromises();

    expect(
      emailField.find('.ant-form-item-feedback-icon-success').exists(),
    ).toBe(true);
  });

  it('verifies the password confirm correctly', async () => {
    const wrapper = mount(CreateUserView, {
      global: {
        provide: {
          [userStoreSymbol as symbol]: useUserStore(),
        },
      },
    });

    const passwordField = wrapper.findAllComponents(FormItem)[3];
    const confirmPasswordField = wrapper.findAllComponents(FormItem)[4];

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
    const testData = ['Name', 'Username', 'E@Ma.il', 'Password', 'Password'];

    const userStore = useUserStore();
    const formStore = useFormStore('createUserForm');

    const wrapper = mount(CreateUserView, {
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

    expect(userStore.createUser).toHaveBeenCalledWith({
      name: 'Name',
      username: 'Username',
      email: 'E@Ma.il',
      password: 'Password',
    });
  });
});
