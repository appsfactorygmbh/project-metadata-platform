import type { ApiInstance, ApiTypes } from '@/api/ApiType.type';
import { getApiConfiguration } from '@/utils/api';
import { type PiniaStore, defineGenericStore } from 'pinia-generic';
import { type AuthStore, useAuthStore } from './AuthStore';
import type { UnwrapRef } from 'vue';
import type { Pinia } from 'pinia';
import { piniaInstance } from './piniaInstance';
import { type CallApiType, callApi } from '@/utils/api';

type ApiStore<Api extends ApiTypes> = PiniaStore<
  'api',
  {
    auth: AuthStore;
    ApiInstance: ApiInstance<Api>;
    api: Api | null;
    isLoading: boolean;
    name: string;
  },
  unknown,
  {
    initApi: () => void;
    setIsLoading: (isLoading: boolean) => void;
    callApi: CallApiType<Api>;
  }
>;

export const useApiStore = <Api extends ApiTypes>(
  ApiInstance: ApiInstance<Api>,
  pinia: Pinia = piniaInstance,
) =>
  defineGenericStore<ApiStore<Api>>({
    state: {
      auth: useAuthStore(pinia),
      ApiInstance,
      api: null,
      isLoading: false,
      name: '',
    },

    actions: {
      initApi() {
        const accessToken = this.auth.authToken;
        if (!accessToken) return;
        const config = getApiConfiguration(accessToken);
        this.api = new ApiInstance(config) as UnwrapRef<Api>;
      },

      setIsLoading(isLoading: boolean) {
        this.isLoading = isLoading;
      },

      async callApi(apiCall, args) {
        try {
          if (!this.api) throw new Error(`${this.name} Api is not set`);
          this.setIsLoading(true);
          return await callApi<Api, typeof apiCall, typeof args>(
            apiCall,
            // @ts-expect-error false positive
            args,
            this.api,
          );
        } finally {
          this.setIsLoading(false);
        }
      },
    },
  });

export type { ApiStore };
