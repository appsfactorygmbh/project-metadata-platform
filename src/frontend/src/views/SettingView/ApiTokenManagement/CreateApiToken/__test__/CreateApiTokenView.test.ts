import { describe, expect, it } from 'vitest';
import { VueWrapper, flushPromises, mount } from '@vue/test-utils';
import { setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import {
  apiTokenRoutingSymbol,
  apiTokenStoreSymbol,
} from '@/store/injectionSymbols';
import { FormItem, Input, Select } from 'ant-design-vue';
import CreateApiTokenView from '../CreateApiTokenView.vue';
import type { ApiTokenModel } from '@/models/ApiToken';
import { useApiTokenStore } from '@/store';
import { useFormStore } from '@/components/Form';

describe('CreateApiTokenView.vue', () => {
  setActivePinia(createTestingPinia({ stubActions: false }));

  let wrapper: VueWrapper;
  afterEach(() => {
    if (wrapper) {
      wrapper.unmount();
    }
  });

  const mockApiTokenRoutingService = {
    setTeamId: vi.fn((id: number) => Promise.resolve()),
  };

  it('renders correctly', () => {
    const inputFields = ['Token Name', 'Scopes'];

    wrapper = mount(CreateApiTokenView, {
      global: {
        provide: {
          [apiTokenStoreSymbol as symbol]: useApiTokenStore(),
          [apiTokenRoutingSymbol as symbol]: mockApiTokenRoutingService,
        },
      },
    });

    const formItems = wrapper.findAllComponents(FormItem);
    expect(formItems).toHaveLength(2);

    const nameInput = wrapper.findComponent(Input);
    expect(nameInput.props('placeholder')).toBe('Token Name');

    const scopeSelect = wrapper.findComponent(Select);
    expect(scopeSelect.props('placeholder')).toBe('Token Scopes');
  });

  it('verifies a valid token name correctly', async () => {
    const testData: ApiTokenModel[] = [
      {
        id: 1,
        name: 'Token Name',
        scopes: [],
        expirationDate: new Date(),
      },
    ];

    wrapper = mount(CreateApiTokenView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            apiToken: {
              apiTokens: testData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [apiTokenStoreSymbol as symbol]: useApiTokenStore(),
          [apiTokenRoutingSymbol as symbol]: mockApiTokenRoutingService,
        },
      },
    });

    const tokenField = wrapper.findAllComponents(FormItem)[0];

    await tokenField.find('.ant-input').setValue('test');
    await flushPromises();

    expect(
      tokenField.find('.ant-form-item-feedback-icon-success').exists(),
    ).toBe(true);
  });

  it('verifies an invalid token name correctly', async () => {
    const testData: ApiTokenModel[] = [
      {
        id: 1,
        name: 'Test Name',
        scopes: [],
        expirationDate: new Date(),
      },
    ];

    wrapper = mount(CreateApiTokenView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
          initialState: {
            apiToken: {
              apiTokens: testData,
            },
          },
        }),
      ],
      global: {
        provide: {
          [apiTokenStoreSymbol as symbol]: useApiTokenStore(),
          [apiTokenRoutingSymbol as symbol]: mockApiTokenRoutingService,
        },
      },
    });

    const tokenNameField = wrapper.findAllComponents(FormItem)[0];

    await tokenNameField.find('.ant-input').setValue('Test Name');
    await flushPromises();

    expect(
      tokenNameField.find('.ant-form-item-feedback-icon-error').exists(),
    ).toBe(true);
  });

  it('submits the form correctly', async () => {
    const tokenStore = useApiTokenStore();
    const formStore = useFormStore('CreateApiTokenForm');
    const createSpy = vi
      .spyOn(tokenStore, 'create')
      .mockImplementation(() => Promise.resolve(1));
    const mockApiTokenRoutingService = {
      setApiTokenId: vi.fn(),
    };
    wrapper = mount(CreateApiTokenView, {
      global: {
        provide: {
          [apiTokenStoreSymbol as symbol]: tokenStore,
          [apiTokenRoutingSymbol as symbol]: mockApiTokenRoutingService,
        },
      },
    });
    const nameInput = wrapper.findComponent(Input);
    nameInput.vm.$emit('update:value', 'Test Token');
    nameInput.vm.$emit('change', 'Test Token');
    await flushPromises();
    const scopeSelect = wrapper.findComponent(Select) as VueWrapper;

    scopeSelect.vm.$emit('update:value', ['SCIM']);
    scopeSelect.vm.$emit('change', ['SCIM']);

    await flushPromises();

    await formStore.submit();
    await flushPromises();

    expect(createSpy).toHaveBeenCalled();

    expect(createSpy).toHaveBeenCalledWith({
      name: 'Test Token',
      scopes: ['SCIM'],
    });
  });
});
