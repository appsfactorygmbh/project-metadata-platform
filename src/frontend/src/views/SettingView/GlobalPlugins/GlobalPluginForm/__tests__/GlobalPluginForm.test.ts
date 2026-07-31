import { describe, it, expect, expectTypeOf } from 'vitest';
import { flushPromises, mount, VueWrapper } from '@vue/test-utils';
import { GlobalPluginForm } from '..';
import type { GlobalPluginFormData } from '..';
import { useFormStore } from '@/components/Form';
import { createPinia, setActivePinia } from 'pinia';

const testForm: GlobalPluginFormData = {
  pluginName: 'testPlugin',
  keys: [
    {
      key: 1721034372425,
      value: 'testValue',
      archived: false,
    },
    {
      key: 1721034372428,
      value: 'testValue2',
      archived: false,
    },
    {
      key: 1721034372429,
      value: 'testValue3',
      archived: false,
    },
  ],
  baseUrl: 'testplugin.de',
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
        initialValues: { pluginName: '', keys: [], baseUrl: '' },
      },
    });
  });

  it('should render a form', () => {
    expect(wrapper.find('form').exists()).toBe(true);
  });

  it.todo('should render the correct number of input fields', () => {
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

  it.todo("should add key fields when 'Add Key' is clicked", async () => {
    const button = wrapper.find('button');
    const count = wrapper.findAll('input').length;
    await button.trigger('click');
    expect(wrapper.findAll('input').length).toBe(count + 1);
    await button.trigger('click');
    expect(wrapper.findAll('input').length).toBe(count + 2);
  });

  it.todo("should remove key fields when 'Remove Key' is clicked", async () => {
    await flushPromises();
    const count = wrapper.findAll('input').length;
    for (let i = 1; i < count; i++) {
      const button = wrapper.findComponent(
        '[data-test="dynamic-delete-button"]',
      );
      await button.trigger('click');
      expect(wrapper.findAll('input').length).toBe(count - i);
    }
  });

  it.todo('should validate the form if pluginName is not set', async () => {
    await flushPromises();
    const input = wrapper.find('[id="form_item_pluginName"]');
    await input.setValue('');
    await flushPromises();

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
    let button = wrapper.find('button');
    await button.trigger('click');
    button = wrapper.find('button');
    await button.trigger('click');
    button = wrapper.find('button');
    await button.trigger('click');

    expect(formStore.validate).rejects.toMatchObject({
      errorFields: [
        {
          errors: ['Please insert the key or remove it.'],
        },
        {
          errors: ['Please insert the key or remove it.'],
        },
        {
          errors: ['Please insert the key or remove it.'],
        },
      ],
    });

    const inputs = wrapper.findAll('input');
    await inputs[1].setValue('testPlugin');
    await inputs[1].setValue('testValue');
    await inputs[2].setValue('testValue2');
    await inputs[3].setValue('testValue3');

    expect(formStore.validate()).resolves.toEqual(formStore.getFieldsValue);
  });
});
