import { describe, expect, it, beforeEach } from 'vitest';
import { flushPromises, mount, VueWrapper } from '@vue/test-utils';
import { LoginForm } from '..';
import { setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import { useFormStore, type FormStore } from '@/components/Form';
import { nanoid } from 'nanoid';
import _ from 'lodash';

const testData = {
  email: 'email@email.email',
  password: 'testpassword',
  remember: true,
};

describe('LoginForm.vue', () => {
  let formStore: FormStore;
  let wrapper: VueWrapper;

  beforeEach(() => {
    setActivePinia(
      createTestingPinia({
        stubActions: false,
      }),
    );
    formStore = useFormStore('testFormStore_' + nanoid());
    wrapper = mount(LoginForm, {
      props: {
        formStore: formStore,
      },
    });
  });

  it('renders correctly', () => {
    expect(wrapper.exists()).toBe(true);
  });

  it('should have a form', () => {
    expect(wrapper.find('form').exists()).toBe(true);
  });

  it('should have a password input', () => {
    expect(wrapper.find('input[id="standard_login_password"]').exists()).toBe(
      true,
    );
  });

  it('should have a login button', () => {
    expect(wrapper.find('button').exists()).toBe(true);
  });

  it('should have a remember me checkbox', () => {
    expect(wrapper.find('input[id="standard_login_remember"]').exists()).toBe(
      true,
    );
  });

  it("should not call the 'login' method when the form is submitted with invalid data", async () => {
    const submit = vi.spyOn(formStore, 'submit');
    const onSubmit = vi.fn(() => {});
    formStore.setOnSubmit(onSubmit);
    const email = wrapper.find('input[id="standard_login_email"]');
    const password = wrapper.find('input[id="standard_login_password"]');
    const remember = wrapper.find('input[id="standard_login_remember"]');

    await email.setValue('');
    await password.setValue('');
    await remember.setValue('');

    expect(formStore.validate()).rejects.toThrow();

    const loginButton = wrapper.find('button');

    await loginButton.trigger('click');

    expect(submit).toHaveBeenCalledTimes(1);
    await flushPromises();

    expect(onSubmit).not.toHaveBeenCalled();
  });

  it("should call the 'login' method when the form is submitted", async () => {
    const submit = vi.spyOn(formStore, 'submit');
    const onSubmit = vi.fn(() => {});
    formStore.setOnSubmit(onSubmit);

    const email = wrapper.find('input[id="standard_login_email"]');
    const password = wrapper.find('input[id="standard_login_password"]');
    const remember = wrapper.find('input[id="standard_login_remember"]');

    await email.setValue(testData.email);
    await password.setValue(testData.password);
    await remember.setValue(testData.remember);

    expect(formStore.validate()).resolves.toStrictEqual(
      _.omit(testData, 'remember'),
    );

    const loginButton = wrapper.find('button');

    await loginButton.trigger('click');

    expect(submit).toHaveBeenCalledTimes(1);
    await flushPromises();

    expect(onSubmit).toHaveBeenCalledTimes(1);
    expect(onSubmit).toHaveBeenCalledWith(testData);
  });
});
