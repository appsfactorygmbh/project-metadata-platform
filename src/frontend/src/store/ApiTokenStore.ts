import type {
  ApiTokenModel,
  CreateApiTokenModel,
  DetailedApiTokenModel,
} from '@/models/ApiToken';
import { type Pinia } from 'pinia';
import { type PiniaStore, useStore } from 'pinia-generic';
import { piniaInstance } from './piniaInstance';
import { type ApiStore, useApiStore } from './ApiStore';
import { AuthApi } from '@/api/generated';

type StoreState = {
  apiTokens: ApiTokenModel[];
  apiToken: DetailedApiTokenModel | null;
  isLoading: boolean;
  isLoadingCreate: boolean;
  isLoadingApiToken: boolean;
  isLoadingApiTokens: boolean;
  isLoadingDelete: boolean;
  isLoadingRegenerate: boolean;
  createdSuccessfully: boolean;
  removedSuccessfully: boolean;
  regeneratedSuccessfully: boolean;
};

type StoreGetters = {
  getApiTokens: () => ApiTokenModel[];
  getApiToken: () => DetailedApiTokenModel | null;
  getIsLoading: () => boolean;
  getIsLoadingCreate: () => boolean;
  getIsLoadingApiToken: () => boolean;
  getIsLoadingApiTokens: () => boolean;
  getIsLoadingDelete: () => boolean;
  getIsLoadingRegenerate: () => boolean;
  getCreatedSuccessfully: () => boolean;
  getRemovedSuccessfully: () => boolean;
  getRegeneratedSuccessfully: () => boolean;
};

type StoreActions = {
  refreshAuth: () => void;
  setApiTokens: (apiTokens: ApiTokenModel[]) => void;
  setApiToken: (apiToken: DetailedApiTokenModel | null) => void;
  setIsLoading: (isLoading: boolean) => void;
  setIsLoadingCreate: (isLoadingCreate: boolean) => void;
  setIsLoadingApiToken: (isLoadingApiToken: boolean) => void;
  setIsLoadingApiTokens: (isLoadingApiTokens: boolean) => void;
  setIsLoadingDelete: (isLoadingDelete: boolean) => void;
  setIsLoadingRegenerate: (isLoadingRegenerate: boolean) => void;
  setCreatedSuccessfully: (createdSuccessfully: boolean) => void;
  setRemovedSuccessfully: (removedSuccessfully: boolean) => void;
  setRegeneratedSuccessfully: (regeneratedSuccessfully: boolean) => void;
  fetchAll: () => Promise<void>;
  fetchApiToken: (tokenId: number) => Promise<void>;
  create: (newToken: CreateApiTokenModel) => Promise<void>;
  regenerate: (tokenId: ApiTokenModel['id']) => Promise<void>;
  delete: (tokenId: ApiTokenModel['id']) => Promise<void>;
};

type Store = PiniaStore<'apiToken', StoreState, StoreGetters, StoreActions>;

export const useApiTokenStore = (pinia: Pinia = piniaInstance): Store => {
  return useStore<Store, ApiStore<AuthApi>>(
    'apiToken',
    {
      state: {
        apiTokens: [],
        apiToken: null,
        isLoadingCreate: false,
        isLoadingApiToken: false,
        isLoadingApiTokens: false,
        isLoadingDelete: false,
        isLoadingRegenerate: false,
        createdSuccessfully: false,
        removedSuccessfully: false,
        regeneratedSuccessfully: false,
      },
      getters: {
        getApiTokens(): ApiTokenModel[] {
          return this.apiTokens;
        },
        getApiToken(): DetailedApiTokenModel | null {
          return this.apiToken;
        },
        getIsLoading(): boolean {
          return this.isLoadingCreate || this.isLoadingApiTokens;
        },
        getIsLoadingCreate(): boolean {
          return this.isLoadingCreate;
        },
        getIsLoadingApiToken(): boolean {
          return this.isLoadingApiToken;
        },
        getIsLoadingApiTokens(): boolean {
          return this.isLoadingApiTokens;
        },
        getIsLoadingDelete(): boolean {
          return this.isLoadingDelete;
        },
        getIsLoadingRegenerate(): boolean {
          return this.isLoadingRegenerate;
        },
        getCreatedSuccessfully(): boolean {
          return this.createdSuccessfully;
        },
        getRemovedSuccessfully(): boolean {
          return this.removedSuccessfully;
        },
        getRegeneratedSuccessfully(): boolean {
          return this.regeneratedSuccessfully;
        },
      },
      actions: {
        refreshAuth(): void {
          this.initApi();
        },
        setApiTokens(apiTokens: ApiTokenModel[]): void {
          this.apiTokens = apiTokens;
        },
        setApiToken(apiToken: DetailedApiTokenModel | null): void {
          this.apiToken = apiToken;
        },
        setIsLoadingCreate(isLoadingCreate: boolean): void {
          this.isLoadingCreate = isLoadingCreate;
        },
        setIsLoadingApiToken(isLoadingApiToken: boolean): void {
          this.isLoadingApiToken = isLoadingApiToken;
        },
        setIsLoadingApiTokens(isLoadingApiTokens: boolean): void {
          this.isLoadingApiTokens = isLoadingApiTokens;
        },
        setIsLoadingDelete(isLoadingDelete: boolean): void {
          this.isLoadingDelete = isLoadingDelete;
        },
        setIsLoadingRegenerate(isLoadingRegenerate: boolean): void {
          this.isLoadingRegenerate = isLoadingRegenerate;
        },
        setCreatedSuccessfully(createdSuccessfully: boolean): void {
          this.createdSuccessfully = createdSuccessfully;
        },
        setRemovedSuccessfully(removedSuccessfully: boolean): void {
          this.removedSuccessfully = removedSuccessfully;
        },
        setRegeneratedSuccessfully(regeneratedSuccessfully: boolean): void {
          this.regeneratedSuccessfully = regeneratedSuccessfully;
        },

        async fetchAll(): Promise<void> {
          this.setIsLoadingApiTokens(true);
          try {
            const tokens: ApiTokenModel[] =
              (await this.callApi('authApiTokensGet', {})) ?? [];
            this.setApiTokens(tokens);
          } finally {
            this.setIsLoadingApiTokens(false);
          }
        },

        async fetchApiToken(tokenId: number): Promise<void> {
          this.setIsLoadingApiToken(true);
          try {
            const token =
              (await this.callApi('authApiTokensTokenIdGet', {
                tokenId: tokenId,
              })) ?? null;
            this.setApiToken(token);
          } finally {
            this.setIsLoadingApiToken(false);
          }
        },

        async create(newToken: CreateApiTokenModel): Promise<void> {
          try {
            this.setIsLoadingCreate(true);
            this.setCreatedSuccessfully(false);
            await this.callApi('authApiTokensPost', {
              createApiTokenRequest: newToken,
            });
            await this.fetchAll();
            this.setCreatedSuccessfully(true);
          } catch (e) {
            this.setCreatedSuccessfully(false);
            throw e;
          } finally {
            this.setIsLoadingCreate(false);
          }
        },

        async regenerate(tokenId: number | null | undefined): Promise<void> {
          try {
            if (!tokenId) {
              return;
            }
            this.setIsLoadingRegenerate(true);
            this.setRegeneratedSuccessfully(false);
            await this.callApi('authApiTokensTokenIdPatch', {
              tokenId: tokenId,
            });
            this.setIsLoadingApiTokens(true);
            this.setIsLoadingApiToken(true);
            this.fetchAll();
            this.setIsLoadingApiToken(false);
            this.setIsLoadingApiTokens(false);
            this.setRegeneratedSuccessfully(true);
          } catch (e) {
            this.setRegeneratedSuccessfully(false);
            throw e;
          } finally {
            this.setIsLoadingRegenerate(false);
          }
        },

        async delete(tokenId: number | null | undefined): Promise<void> {
          try {
            if (!tokenId) {
              return;
            }
            this.setIsLoadingDelete(true);
            this.setRemovedSuccessfully(false);
            await this.callApi('authApiTokensTokenIdDelete', {
              tokenId: tokenId,
            });
            this.setRemovedSuccessfully(true);
            this.fetchAll();
          } catch (e) {
            this.setRemovedSuccessfully(false);
            throw e;
          } finally {
            this.setIsLoadingDelete(false);
          }
        },
      },
    },
    useApiStore(AuthApi, pinia),
  )(pinia);
};

type ApiTokenStore = ReturnType<typeof useApiTokenStore>;
export type { ApiTokenStore };
