import { AuthorizationApi } from '@/api/generated';
import type { PermissionListModel } from '@/models/Authorization';
import type { Pinia } from 'pinia';
import { type PiniaStore, useStore } from 'pinia-generic';
import { type ApiStore, useApiStore } from './ApiStore';
import { piniaInstance } from './piniaInstance';

type StoreState = {
  resources: string[];
  permissionFilters: PermissionListModel | null;
  isLoading: boolean;
  isLoadingResources: boolean;
  isLoadingPermissionFilters: boolean;
};

type StoreGetters = {
  getResources: () => string[];
  getPermissionFilters: () => PermissionListModel | null;
  getIsLoading: () => boolean;
  getIsLoadingResources: () => boolean;
  getIsLoadingPermissionFilters: () => boolean;
};

type StoreActions = {
  refreshAuth: () => void;
  setResources: (resources: string[]) => void;
  setPermissionFilters: (permissions: PermissionListModel | null) => void;
  setIsLoading: (isLoading: boolean) => void;
  setIsLoadingResources: (isLoadingResources: boolean) => void;
  setIsLoadingPermissionFilters: (isLoadingPermissionFilters: boolean) => void;
  fetchResources: () => Promise<void>;
  fetchPermissions: (resourceKind: string) => Promise<void>;
};

type Store = PiniaStore<
  'authorization',
  StoreState,
  StoreGetters,
  StoreActions
>;

export const useAuthorizationStore = (pinia: Pinia = piniaInstance): Store => {
  return useStore<Store, ApiStore<AuthorizationApi>>(
    'authorization',
    {
      state: {
        resources: [],
        permissionFilters: null,
        isLoading: false,
        isLoadingResources: false,
        isLoadingPermissionFilters: false,
      },
      getters: {
        getResources(): string[] {
          return this.resources;
        },
        getPermissionFilters(): PermissionListModel | null {
          return this.permissionFilters;
        },
        getIsLoading(): boolean {
          return this.isLoadingResources || this.isLoadingPermissionFilters;
        },
        getIsLoadingResources(): boolean {
          return this.isLoadingResources;
        },
        getIsLoadingPermissionFilters(): boolean {
          return this.isLoadingPermissionFilters;
        },
      },
      actions: {
        refreshAuth(): void {
          this.initApi();
        },
        setResources(resources: string[]): void {
          this.resources = resources;
        },
        setPermissionFilters(permissions: PermissionListModel | null): void {
          this.permissionFilters = permissions;
        },
        setIsLoadingResources(isLoadingResources): void {
          this.isLoadingResources = isLoadingResources;
        },
        setIsLoadingPermissionFilters(isLoadingPermissionFilters): void {
          this.isLoadingPermissionFilters = isLoadingPermissionFilters;
        },

        async fetchResources(): Promise<void> {
          this.isLoadingResources = true;
          try {
            const resources: string[] = await this.callApi(
              'authorizationResourcesGet',
              {},
            );
            resources.sort();
            this.setResources(resources);
          } finally {
            this.isLoadingResources = false;
          }
        },

        async fetchPermissions(resourceKind: string): Promise<void> {
          this.isLoadingPermissionFilters = true;
          try {
            const permissionFilters: PermissionListModel =
              (await this.callApi('authorizationResourceKindGet', {
                resourceKind: resourceKind,
              })) ?? null;
            this.setPermissionFilters(permissionFilters);
          } finally {
            this.isLoadingPermissionFilters = false;
          }
        },
      },
    },
    useApiStore(AuthorizationApi, pinia),
  )(pinia);
};

type AuthorizationStore = ReturnType<typeof useAuthorizationStore>;
export type { AuthorizationStore };
