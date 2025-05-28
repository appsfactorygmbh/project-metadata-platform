import type { ApiInstance, ApiTypes } from '@/api/ApiType.type';
import { callApi, getApiConfiguration, handleFetchError } from '@/utils/api';
import { type PiniaStore, defineGenericStore } from 'pinia-generic';
import { type AuthStore, useAuthStore } from './AuthStore';
import { type UnwrapRef } from 'vue';
import type { Pinia } from 'pinia';
import { piniaInstance } from './piniaInstance';
import type { GenericStore } from '@/utils/store';

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
    callApi: <
      Endpoint extends keyof Api,
      //@ts-expect-error function type is not inferred correctly
      Args extends Parameters<Api[Endpoint]>[0],
    >(
      endpoint: Endpoint,
      args: Args,
      //@ts-expect-error function type is not inferred correctly
    ) => ReturnType<Api[Endpoint]>;
  }
>;

export const useApiStore = <Api extends ApiTypes>(
  ApiInstance: ApiInstance<Api>,
  pinia: Pinia = piniaInstance,
): GenericStore<ApiStore<Api>> =>
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

      //@ts-expect-error return type is always a Promise but type is not inferred correctly
      async callApi(apiCall, args) {
        try {
          if (!this.api) throw new Error(`${this.name} Api is not set`);
          this.setIsLoading(true);
          return await callApi<Api, typeof apiCall, typeof args>(
            apiCall,
            // @ts-expect-error false positive
            args,
            this.api as Api,
          );
        } catch (error: unknown) {
          await handleFetchError(error);
        } finally {
          this.setIsLoading(false);
        }
      },
    },
  });

export type { ApiStore };
