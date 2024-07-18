import { describe, expect, it, beforeEach } from 'vitest';
import { flushPromises, mount, VueWrapper } from '@vue/test-utils';
import { LoginForm } from '..';
import { setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import { useFormStore, type FormStore } from '@/components/Form';
import _ from 'lodash';

const testData = {
  username: 'testuser',
  password: 'testpassword',
  remember: false,
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
    formStore = useFormStore('testFormStore');
    wrapper = mount(LoginForm, {
      props: {
        formStore: formStore,
      },
    });
  });

  it('renders correctly', () => {
    expect(wrapper.exists()).toBe(true);
  });

  it("should call the 'login' method when the form is submitted", async () => {
    const submit = vi.spyOn(formStore, 'submit');
    const onSubmit = vi.fn(() => {});
    formStore.setOnSubmit(onSubmit);
    const username = wrapper.find('input[id="standard_login_username"]');
    const password = wrapper.find('input[id="standard_login_password"]');

    await username.setValue(testData.username);
    await password.setValue(testData.password);

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
