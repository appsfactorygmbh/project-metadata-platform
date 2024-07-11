import { describe, it, expect, expectTypeOf } from 'vitest';
import { mount, VueWrapper } from '@vue/test-utils';
import { GlobalPluginForm, type GlobalPluginFormData } from '..';
import { useFormStore } from '@/components/Form';
import { createPinia, setActivePinia } from 'pinia';

const testForm: GlobalPluginFormData = {
  pluginName: 'testPlugin',
  keys: [
    {
      key: 1231231203,
      value: 'testValue',
    },
    {
      key: 1231231203,
      value: 'testValue2',
    },
    {
      key: 1231231203,
      value: 'testValue3',
    },
  ],
};

describe('GlobalPluginForm.vue', () => {
  let wrapper: VueWrapper;
  let formStore: ReturnType<typeof useFormStore>;

  beforeEach(() => {
    setActivePinia(createPinia());
    formStore = useFormStore('testForm');
    formStore.resetFields();

    wrapper = mount(GlobalPluginForm, {
      props: {
        formStore,
        initialValues: testForm,
      },
    });
  });

  it('should export a valid component', () => {
    expect(GlobalPluginForm).not.toBeUndefined();
    expectTypeOf(GlobalPluginForm).toMatchTypeOf<import('vue').Component>();
  });

  it("should render an empty form if there's no initialValues", () => {
    setActivePinia(createPinia());
    const formStore = useFormStore('testForm');

    wrapper = mount(GlobalPluginForm, {
      props: {
        formStore,
        initialValues: { pluginName: '', keys: [] },
      },
    });
  });

  it('should render a form', () => {
    expect(wrapper.find('form').exists()).toBe(true);
  });

  it('should render the correct number of input fields', () => {
    expect(wrapper.findAll('input').length).toBe(4);
  });

  it("should effect the form store's values", async () => {
    const input = wrapper.find('input');
    await input.setValue('test');
    expect(formStore.getFieldValue('pluginName')).toBe('test');
  });

  it('should reset the form', async () => {
    const input = wrapper.find('input');
    await input.setValue('test');
    expect(formStore.getFieldValue('pluginName')).toBe('test');

    await formStore.resetFields();
    expect(formStore.getFieldValue('pluginName')).toBe(undefined);
  });

  it("should add key fields when 'Add Key' is clicked", async () => {
    const button = wrapper.find('button');
    const count = wrapper.findAll('input').length;
    await button.trigger('click');
    expect(wrapper.findAll('input').length).toBe(count + 1);
  });

  it.todo("should remove key fields when 'Remove Key' is clicked", async () => {
    const button = wrapper.find('button');
    await button.trigger('click');
    const count = wrapper.findAll('input').length;
    await button.trigger('click');
    expect(wrapper.findAll('input').length).toBe(count - 1);
  });

  it('should validate the form if pluginName is not set', async () => {
    const input = wrapper.find('input');
    await input.setValue('');
    expect(formStore.validate()).rejects.toMatchObject({
      errorFields: [
        {
          errors: ['Please insert the plugin name.'],
          name: 'pluginName',
          warnings: [],
        },
      ],
    });

    await input.setValue('testPlugin');
    expect(formStore.validate()).resolves.toEqual(testForm);
  });

  it.todo('should validate the form if keys are not set', async () => {
    const button = wrapper.find('button');
    await button.trigger('click');
    await button.trigger('click');
    await button.trigger('click');

    expect(formStore.validate()).rejects.toMatchObject({
      errorFields: [
        {
          errors: ['Please insert the key.'],
          name: 'keys[0].key',
          warnings: [],
        },
        {
          errors: ['Please insert the value.'],
          name: 'keys[0].value',
          warnings: [],
        },
        {
          errors: ['Please insert the key.'],
          name: 'keys[1].key',
          warnings: [],
        },
        {
          errors: ['Please insert the value.'],
          name: 'keys[1].value',
          warnings: [],
        },
        {
          errors: ['Please insert the key.'],
          name: 'keys[2].key',
          warnings: [],
        },
        {
          errors: ['Please insert the value.'],
          name: 'keys[2].value',
          warnings: [],
        },
      ],
    });

    const inputs = wrapper.findAll('input');
    await inputs[0].setValue('1231231203');
    await inputs[1].setValue('testValue');
    await inputs[2].setValue('1231231203');
    await inputs[3].setValue('testValue2');
    await inputs[4].setValue('1231231203');
    await inputs[5].setValue('testValue3');

    expect(formStore.validate()).resolves.toEqual(formStore.getFieldsValue);
  });
});
