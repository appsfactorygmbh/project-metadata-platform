import type { PluginModel } from '@/models/Plugin';
import { type PiniaStore, useStore } from 'pinia-generic';
import { ProjectsApi } from '@/api/generated';
import { type ApiStore, useApiStore } from './ApiStore';
import type { ProjectModel } from '@/models/Project';
import { piniaInstance } from './piniaInstance';
import type { Pinia } from 'pinia';

type StoreState = {
  plugins: PluginModel[];
  isLoadingPlugins: boolean;
  unarchivedPlugins: PluginModel[];
};

type StoreGetters = {
  getPlugins: () => PluginModel[];
  getIsLoading: () => boolean;
  getUnarchivedPlugins: () => PluginModel[];
};

type StoreActions = {
  refreshAuth: () => void;
  setPlugins: (plugins: PluginModel[]) => void;
  setLoadingPlugins: (status: boolean) => void;
  fetch: (projectID: number) => Promise<void>;
  fetchUnarchivedPlugins: (projectID: number) => Promise<void>;
  setUnarchivedPlugins: (plugins: PluginModel[]) => void;
};

type Store = PiniaStore<'plugin', StoreState, StoreGetters, StoreActions>;

export const usePluginStore = (pinia: Pinia = piniaInstance): Store => {
  return useStore<Store, ApiStore<ProjectsApi>>(
    'plugin',
    {
      state: {
        plugins: [],
        isLoadingPlugins: false,
      },

      getters: {
        getPlugins(): PluginModel[] {
          return this.plugins;
        },
        getIsLoading(): boolean {
          return this.isLoadingPlugins;
        },
        getUnarchivedPlugins(): PluginModel[] {
          return this.unarchivedPlugins;
        },
      },

      actions: {
        refreshAuth(): void {
          this.initApi();
        },
        setPlugins(plugins: PluginModel[]): void {
          this.plugins = plugins;
        },
        setLoadingPlugins(status: boolean): void {
          this.isLoadingPlugins = status;
        },
        setUnarchivedPlugins(plugins: PluginModel[]): void {
          this.unarchivedPlugins = plugins;
        },
        async fetch(id: ProjectModel['id']) {
          try {
            this.setLoadingPlugins(true);
            const plugins: PluginModel[] = await this.callApi(
              'projectsIdPluginsGet',
              {
                id,
              },
            );
            this.setPlugins(plugins);
          } finally {
            this.setLoadingPlugins(false);
          }
          console.log(this.getPlugins);
        },
        async fetchUnarchivedPlugins(projectID: number) {
          try {
            this.setLoadingPlugins(true);
            const plugins: PluginModel[] = await this.callApi(
              // TODO: change to unarchived
              'projectsIdPluginsGet',
              {
                id: projectID,
              },
            );
            this.setUnarchivedPlugins(plugins);
          } finally {
            this.setLoadingPlugins(false);
          }
        },
      },
    },
    useApiStore(ProjectsApi, pinia),
  )(pinia);
};

type PluginStore = ReturnType<typeof usePluginStore>;
export type { PluginStore };
