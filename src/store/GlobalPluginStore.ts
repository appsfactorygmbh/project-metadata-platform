import type { GlobalPluginModel } from '@/models/Plugin';
import { type PiniaStore, useStore } from 'pinia-generic';
import type { ApiStore } from './ApiStore';
import { PluginsApi } from '@/api/generated';
import { useApiStore } from './ApiStore';
import { piniaInstance } from './piniaInstance';
import type { Pinia } from 'pinia';

type StoreState = {
  globalPlugins: GlobalPluginModel[];
  isLoadingGlobalPlugins: boolean;
  isLoadingDelete: boolean;
  removedSuccessfully: boolean;
};

type StoreActions = {
  refreshAuth: () => void;
  fetchAll: () => Promise<void>;
  fetch: (pluginId: number) => Promise<GlobalPluginModel | undefined>;
  create: (plugin: Omit<GlobalPluginModel, 'id'>) => Promise<void>;
  update: (plugin: GlobalPluginModel) => Promise<void>;
  delete: (pluginId: GlobalPluginModel['id']) => Promise<void>;
  setGlobalPlugins: (globalPlugins: GlobalPluginModel[]) => void;
  setLoadingGlobalPlugins: (status: boolean) => void;
  setLoadingDelete: (status: boolean) => void;
  setRemovedSuccessfully: (status: boolean) => void;
};

type StoreGetters = {
  getGlobalPlugins: () => GlobalPluginModel[];
  getIsLoadingGlobalPlugins: () => boolean;
  getIsLoadingDelete: () => boolean;
  getRemovedSuccessfully: () => boolean;
};

type Store = PiniaStore<'globalPlugin', StoreState, StoreGetters, StoreActions>;

export const useGlobalPluginsStore = (pinia: Pinia = piniaInstance) => {
  return useStore<Store, ApiStore<PluginsApi>>(
    'globalPlugin',
    {
      state: {
        globalPlugins: [],
        isLoadingGlobalPlugins: false,
        isLoadingDelete: false,
        removedSuccessfully: true,
      },

      getters: {
        getGlobalPlugins(): GlobalPluginModel[] {
          return this.globalPlugins;
        },
        getIsLoadingGlobalPlugins(): boolean {
          return this.isLoadingGlobalPlugins;
        },
        getIsLoadingDelete(): boolean {
          return this.isLoadingDelete;
        },
        getRemovedSuccessfully(): boolean {
          return this.removedSuccessfully;
        },
      },

      actions: {
        refreshAuth(): void {
          this.initApi();
        },

        setGlobalPlugins(globalPlugins: GlobalPluginModel[]): void {
          this.globalPlugins = globalPlugins;
        },
        setLoadingGlobalPlugins(status: boolean): void {
          this.isLoadingGlobalPlugins = status;
        },
        setLoadingDelete(status: boolean): void {
          this.isLoadingDelete = status;
        },
        setRemovedSuccessfully(status: boolean): void {
          this.removedSuccessfully = status;
        },

        async fetchAll() {
          try {
            this.setLoadingGlobalPlugins(true);
            const globalPlugins: GlobalPluginModel[] = await this.callApi(
              'pluginsGet',
              {},
            );
            this.setGlobalPlugins(globalPlugins);
          } finally {
            this.setLoadingGlobalPlugins(false);
          }
        },

        async fetch(pluginId: number) {
          if (this.globalPlugins.length === 0) {
            await this.fetchAll();
          }
          return this.globalPlugins.find((plugin) => plugin.id === pluginId);
        },

        async delete(pluginId: number) {
          this.setLoadingDelete(true);
          this.setRemovedSuccessfully(false);
          await this.callApi('pluginsPluginIdDelete', {
            pluginId,
          })
            .catch(() => {
              this.setRemovedSuccessfully(false);
            })
            .finally(() => {
              this.setLoadingDelete(false);
            });
        },

        async create(plugin: Omit<GlobalPluginModel, 'id'>) {
          try {
            const response = await this.callApi('pluginsPut', {
              createPluginRequest: {
                ...plugin,
                pluginName: plugin.name,
              },
            });
            if (response) {
              this.fetchAll();
            }
          } catch (err) {
            console.error('Error creating global plugin: ' + err);
          }
        },

        async update(plugin: GlobalPluginModel) {
          try {
            const response = await this.callApi('pluginsPut', {
              createPluginRequest: {
                ...plugin,
                pluginName: plugin.name,
              },
            });
            if (response) {
              this.fetchAll();
            } else {
              throw new Error('Failed to update global plugin');
            }
          } catch (err) {
            console.error('Error updating global plugin:', err);
            throw err;
          }
        },
      },
    },
    useApiStore(PluginsApi, pinia),
  )(pinia);
};

type GlobalPluginsStore = ReturnType<typeof useGlobalPluginsStore>;
export type { GlobalPluginsStore };
