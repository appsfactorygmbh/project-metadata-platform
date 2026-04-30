import { mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';
import { createTestingPinia } from '@pinia/testing';
import {
  apiTokenRoutingSymbol,
  apiTokenStoreSymbol,
} from '@/store/injectionSymbols';
import { useApiTokenStore } from '@/store';
import router from '@/router';
import type { ApiTokenModel } from '@/models/ApiToken';
import { useApiTokenRouting } from '@/utils/hooks';
import { ApiTokenInformationView } from '..';

const tokenData1: ApiTokenModel = {
  id: 100,
  name: 'Token1',
  scopes: ['SCIM'],
  expirationDate: new Date(),
};

const mockRoute = {
  path: '/mock-path',
  query: { tokenId: '200' },
  params: {},
  hash: '',
  fullPath: '/mock-path',
  matched: [],
  meta: {},
  redirectedFrom: undefined,
};
const mockRouter = {
  push: vi.fn(),
};

vi.mock('vue-router', async (importOriginal) => {
  const actual = await importOriginal<typeof import('vue-router')>();
  return {
    ...actual,
    useRoute: () => mockRoute,
    useRouter: () => mockRouter,
  };
});

describe('ApiTokenInformationView.vue', () => {
  setActivePinia(createPinia());
  const tokenStore = useApiTokenStore();

  const generateWrapper = () => {
    return mount(ApiTokenInformationView, {
      plugins: [
        createTestingPinia({
          stubActions: false,
        }),
      ],
      global: {
        provide: {
          [apiTokenStoreSymbol as symbol]: tokenStore,
          [apiTokenRoutingSymbol as symbol]: useApiTokenRouting(),
        },
        plugins: [router],
      },
    });
  };

  it('renders correctly', () => {
    mockRoute.query.tokenId = '100';
    tokenStore.setApiToken(tokenData1);
    const wrapper = generateWrapper();

    expect(wrapper.find('.avatar').exists()).toBe(true);
    expect(wrapper.find('.label').exists()).toBe(true);
    const text = wrapper.findAll('.text');
    expect(text[0].text()).toBe(tokenData1.name);
    expect(text[1].text()).toBe(tokenData1.scopes[0]);
    expect(text[2].text()).toBe(new Date().toLocaleDateString());
  });

  it('displays the token modal when a new token is generated and clears it on confirmation', async () => {
    mockRoute.query.tokenId = '100';
    tokenStore.setApiToken(tokenData1);
    const wrapper = generateWrapper();

    tokenStore.setTokenValue('super-secret-token-123');

    await nextTick();

    expect(document.body.innerHTML).toContain('Copy your token now');
    expect(document.body.innerHTML).toContain('super-secret-token-123');

    const okButton = wrapper
      .findAllComponents({ name: 'AButton' })
      .find((btn) => btn.text() === 'Ok');
    expect(okButton).toBeDefined();

    await okButton!.trigger('click');
    await nextTick();

    expect(tokenStore.getTokenValue).toBeNull();
  });
});
