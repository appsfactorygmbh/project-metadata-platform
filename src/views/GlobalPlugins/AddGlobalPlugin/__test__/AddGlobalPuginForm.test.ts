import { describe, it, expect, expectTypeOf } from 'vitest';
import { mount, VueWrapper } from '@vue/test-utils';
import { useFormStore } from '@/components/Form';
import { setActivePinia } from 'pinia';
import type { AddGlobalPluginFormData } from '@/views/GlobalPlugins/AddGlobalPlugin/AddGlobalPluginFormData.ts';
import { AddGlobalPluginForm } from '@/views/GlobalPlugins/AddGlobalPlugin';
import { pluginStoreSymbol } from '@/store/injectionSymbols.ts';
import { createTestingPinia } from '@pinia/testing';

const testForm: AddGlobalPluginFormData = {
  pluginName: 'testPlugin',
  pluginUrl: 'testUrl',
  globalPlugin: 'testGlobalPlugin',
  inputsDisabled: false
};

describe('AddGlobalPluginForm.vue', () => {
  let wrapper: VueWrapper;
  let formStore: ReturnType<typeof useFormStore>;
  let pluginStoreMock: any;

  beforeEach(() => {
    setActivePinia(createTestingPinia({ createSpy: vi.fn }));

    formStore = useFormStore('testForm');
    formStore.resetFields();

    pluginStoreMock = {
      setLoading: vi.fn(),
      fetchGlobalPlugins: vi.fn().mockResolvedValue([]),
      getGlobalPlugins: []
    };

    wrapper = mount(AddGlobalPluginForm, {
      global: {
        provide: {
          [pluginStoreSymbol as symbol]: pluginStoreMock
        }
      },
      props: {
        formStore,
        initialValues: testForm,
      },
    });
  });

  it('should export a valid component', () => {
    expect(AddGlobalPluginForm).not.toBeUndefined();
    expectTypeOf(AddGlobalPluginForm).toMatchTypeOf<import('vue').Component>();
  });

  it('should render correctly', () => {
    expect(wrapper.html()).toMatchSnapshot();
  });

  it("should render an empty form if there's no initialValues", () => {
    const formStore = useFormStore('testForm');

    wrapper = mount(AddGlobalPluginForm, {
      props: {
        formStore,
        initialValues: { pluginName: '', pluginUrl: '', globalPlugin: '', inputsDisabled: true },
      },
    });
  });

  it('should render a form', () => {
    expect(wrapper.find('form').exists()).toBe(true);
  });

  it('should render the correct number of input fields', () => {
    expect(wrapper.findAll('input').length).toBe(3);
  });

  it("should effect the form store's values", async () => {
    const input = wrapper.find('#inputAddPluginPluginName');
    await input.setValue('test');
    expect(formStore.getFieldValue('pluginName')).toBe('test');
  });

  it('should reset the form', async () => {
    const input = wrapper.find('#inputAddPluginPluginName');
    await input.setValue('test');
    expect(formStore.getFieldValue('pluginName')).toBe('test');

    await formStore.resetFields();
    expect(formStore.getFieldValue('pluginName')).toBe(undefined);
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
});
