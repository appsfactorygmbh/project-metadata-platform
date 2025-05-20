import type {
  GlobalPluginModel,
  CreateGlobalPluginModel,
  PatchGlobalPluginModel,
} from '@/models/GlobalPlugin';
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
  create: (plugin: CreateGlobalPluginModel) => Promise<void>;
  update: (
    pluginId: GlobalPluginModel['id'],
    payload: PatchGlobalPluginModel,
  ) => Promise<GlobalPluginModel>;
  delete: (pluginId: GlobalPluginModel['id']) => Promise<void>;
  findPlugin: (
    id: GlobalPluginModel['id'],
  ) => Promise<GlobalPluginModel | null>;
  setGlobalPlugins: (globalPlugins: GlobalPluginModel[]) => void;
  setLoadingGlobalPlugins: (status: boolean) => void;
  setLoadingDelete: (status: boolean) => void;
  setRemovedSuccessfully: (status: boolean) => void;
  archive: (pluginId: GlobalPluginModel['id']) => Promise<void>;
  unarchive: (pluginId: GlobalPluginModel['id']) => Promise<void>;
};

type StoreGetters = {
  getGlobalPlugins: () => GlobalPluginModel[];
  getIsLoadingGlobalPlugins: () => boolean;
  getIsLoadingDelete: () => boolean;
  getRemovedSuccessfully: () => boolean;
};

type Store = PiniaStore<'globalPlugin', StoreState, StoreGetters, StoreActions>;

export const useGlobalPluginStore = (pinia: Pinia = piniaInstance): Store => {
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

        async findPlugin(id: GlobalPluginModel['id']) {
          let plugin =
            this.globalPlugins.find((plugin) => plugin.id === id) ?? null;
          if (!plugin) plugin = (await this.fetch(id)) ?? null;
          return plugin;
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
              this.setRemovedSuccessfully(true);
              this.setLoadingDelete(false);
            });
        },

        async create(plugin) {
          console.log('creating plugin in store', plugin);
          try {
            const response = await this.callApi('pluginsPut', {
              createPluginRequest: {
                ...plugin,
                baseUrl: plugin.baseUrl ?? '',
              },
            });
            if (response) {
              this.fetchAll();
            }
          } finally {
            this.setIsLoading(false);
          }
        },

        async update(
          pluginId: GlobalPluginModel['id'],
          payload: PatchGlobalPluginModel | { isArchived: boolean },
        ) {
          try {
            const response = await this.callApi('pluginsPluginIdPatch', {
              pluginId,
              patchGlobalPluginRequest: {
                ...payload,
              },
            });
            if (response) {
              this.fetchAll();
              return response;
            } else {
              throw new Error('Failed to update global plugin');
            }
          } catch (err) {
            console.error('Error updating global plugin:', err);
            throw err;
          }
        },
        async archive(pluginId: GlobalPluginModel['id']) {
          this.setLoadingDelete(true);
          this.setRemovedSuccessfully(false);
          const plugin = await this.findPlugin(pluginId);
          if (!plugin) {
            this.setRemovedSuccessfully(false);
            throw new Error(`Plugin with id ${pluginId} not found`);
          }
          await this.update(plugin.id, {
            isArchived: true,
          }).catch((e) => {
            this.setRemovedSuccessfully(false);
            throw e;
          });
          this.setRemovedSuccessfully(true);
          this.setLoadingDelete(false);
          this.fetchAll();
        },

        async unarchive(pluginId: GlobalPluginModel['id']) {
          this.setLoadingGlobalPlugins(true);
          const plugin = await this.findPlugin(pluginId);
          if (!plugin) throw new Error(`Plugin with id ${pluginId} not found`);
          await this.update(plugin.id, {
            isArchived: false,
          });
          this.setLoadingGlobalPlugins(false);
          this.fetchAll();
        },
      },
    },
    useApiStore(PluginsApi, pinia),
  )(pinia);
};

type GlobalPluginStore = ReturnType<typeof useGlobalPluginStore>;
export type { GlobalPluginStore };
