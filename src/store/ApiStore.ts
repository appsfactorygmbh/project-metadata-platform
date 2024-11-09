import type { ApiInstance, ApiTypes } from '@/api/ApiType.type';
import { callApi, getApiConfiguration } from '@/utils/api';
import { type PiniaStore, defineGenericStore } from 'pinia-generic';
import { type AuthStore, authStore } from './AuthStore';
import type { UnwrapRef } from 'vue';

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
      // @ts-expect-error false positive
      Args extends Parameters<Api[Endpoint]>[0],
    >(
      apiCall: Endpoint,
      args: Args extends undefined ? never : Args,
      // @ts-expect-error false positive
    ) => Promise<ReturnType<Api[Endpoint]>>;
  }
>;

export function apiStore<Api extends ApiTypes>(ApiInstance: ApiInstance<Api>) {
  return defineGenericStore<ApiStore<Api>>({
    state: {
      auth: authStore,
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
}

export type { ApiStore };
