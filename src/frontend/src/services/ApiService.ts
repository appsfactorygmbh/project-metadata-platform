import type { ApiInstance, ApiTypes } from '@/api/ApiType.type';
import { callApi, getApiConfiguration } from '@/utils/api';

class ApiService<T extends ApiTypes> {
  api: T | null = null;
  authToken: string | null = null;
  isLoading = false;
  name: string;

  constructor(name: string) {
    this.name = name;
  }

  setAuth = (auth: string | null) => {
    if (!auth) return;
    this.authToken = auth.split('|')[0];
  };

  setApi = (api: T) => {
    this.api = api;
  };

  initApi = (accessToken: string | null, ApiInstance: ApiInstance<T>) => {
    if (!accessToken) return;
    const config = getApiConfiguration(accessToken);
    const api = new ApiInstance(config);
    this.setApi(api as T);
  };

  setIsLoading = (isLoading: boolean) => {
    this.isLoading = isLoading;
  };

  get authHeader(): HeadersInit {
    return this.authToken ? { Authorization: `Bearer ${this.authToken}` } : {};
  }

  callApi = async <
    Api extends ApiTypes = T,
    // @ts-expect-error marks as error but works
    Endpoint extends keyof Api = Api,
    // @ts-expect-error marks as error but works
    Args extends Parameters<Api[Endpoint]>[0] = Parameters<
      // @ts-expect-error marks as error but works
      Api[Endpoint]
    >[0],
  >(
    apiCall: Endpoint,
    args: Args extends undefined ? never : Args,
  ) => {
    try {
      if (!this.api) throw new Error(`${this.name} Api is not set`);
      this.setIsLoading(true);
      // @ts-expect-error marks as error but works
      return await callApi<Api, Endpoint, Args>(apiCall, args, this.api);
    } finally {
      this.setIsLoading(false);
    }
  };
}

export { ApiService };
