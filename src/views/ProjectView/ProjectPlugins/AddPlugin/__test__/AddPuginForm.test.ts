import { describe, expect, expectTypeOf, it } from 'vitest';
import {
  VueWrapper,
  enableAutoUnmount,
  flushPromises,
  mount,
} from '@vue/test-utils';
import { useFormStore } from '@/components/Form';
import { setActivePinia } from 'pinia';
import type { AddPluginFormData } from '../AddPluginFormData';
import { AddPluginForm } from '..';
import { pluginStoreSymbol } from '@/store/injectionSymbols.ts';
import { createTestingPinia } from '@pinia/testing';

const testForm: AddPluginFormData = {
  pluginName: 'testPlugin',
  pluginUrl: 'testUrl',
  globalPlugin: 'testGlobalPlugin',
  inputsDisabled: false,
};

describe('AddPluginForm.vue', () => {
  let wrapper: VueWrapper;
  let formStore: ReturnType<typeof useFormStore>;
  let pluginStoreMock: unknown;

  enableAutoUnmount(afterEach);

  beforeEach(() => {
    setActivePinia(
      createTestingPinia({ createSpy: vi.fn, stubActions: false }),
    );

    formStore = useFormStore('testForm');
    formStore.resetFields();

    pluginStoreMock = {
      setLoading: vi.fn(),
      fetchGlobalPlugins: vi.fn().mockResolvedValue([]),
      getGlobalPlugins: [],
    };

    wrapper = mount(AddPluginForm, {
      global: {
        provide: {
          [pluginStoreSymbol as symbol]: pluginStoreMock,
        },
      },
      props: {
        formStore,
        initialValues: testForm,
      },
    });
  });

  it('should export a valid component', () => {
    expect(AddPluginForm).not.toBeUndefined();
    expectTypeOf(AddPluginForm).toMatchTypeOf<import('vue').Component>();
  });

  it("should render an empty form if there's no initialValues", () => {
    const formStore = useFormStore('testForm');

    wrapper = mount(AddPluginForm, {
      props: {
        formStore,
        initialValues: {
          pluginName: '',
          pluginUrl: '',
          globalPlugin: '',
          inputsDisabled: true,
        },
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
    await flushPromises();
    expect(formStore.getFieldValue('pluginName')).toBe('test');
  });

  it('should reset the form', async () => {
    const input = wrapper.find('#inputAddPluginPluginName');
    await input.setValue('test');
    expect(formStore.getFieldValue('pluginName')).toBe('test');

    await formStore.resetFields();
    expect(formStore.getFieldValue('pluginName')).toBe(undefined);
  });

  it('should disable inputs', async () => {
    wrapper = mount(AddPluginForm, {
      props: {
        formStore,
        initialValues: {
          pluginName: '',
          pluginUrl: '',
          globalPlugin: '',
          inputsDisabled: true,
        },
      },
    });

    const inputPluginName = wrapper.find('#inputAddPluginPluginName');
    const inputPluginUrl = wrapper.find('#inputAddPluginPluginUrl');
    expect(inputPluginName.attributes('disabled')).toBe('');
    expect(inputPluginUrl.attributes('disabled')).toBe('');
  });

  it('should enable inputs after plugin select', async () => {
    const inputPluginName = wrapper.find('#inputAddPluginPluginName');
    const inputPluginUrl = wrapper.find('#inputAddPluginPluginUrl');
    expect(inputPluginName.attributes('disabled')).toBe(undefined);
    expect(inputPluginUrl.attributes('disabled')).toBe(undefined);
  });

  it('should validate the form if pluginName is not set', async () => {
    const formStore2: ReturnType<typeof useFormStore> =
      useFormStore('testForm3');

    wrapper = mount(AddPluginForm, {
      global: {
        provide: {
          [pluginStoreSymbol as symbol]: pluginStoreMock,
        },
      },
      props: {
        formStore: formStore2,
        initialValues: {
          pluginName: '',
          pluginUrl: 'testUrl',
          globalPlugin: 'testGlobalPlugin',
          inputsDisabled: false,
        },
      },
    });

    expect(formStore2.validate()).rejects.toMatchObject({
      errorFields: [
        {
          errors: ['Please insert the plugin name.'],
          name: 'pluginName',
          warnings: [],
        },
      ],
    });
  });
});
