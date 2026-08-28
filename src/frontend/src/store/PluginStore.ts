import type {
  CreatePluginModel,
  PluginEditModel,
  PluginListModel,
  PluginModel,
} from '@/models/Plugin';
import { type PiniaStore, useStore } from 'pinia-generic';
import { ProjectPluginsApi } from '@/api/generated';
import { type ApiStore, useApiStore } from './ApiStore';
import type { ProjectModel } from '@/models/Project';
import { piniaInstance } from './piniaInstance';
import type { Pinia } from 'pinia';
import type { ResourceActions } from '@/models/utils';

type StoreState = {
  plugins: PluginModel[];
  isLoadingPlugins: boolean;
  unarchivedPlugins: PluginModel[];
};

type StoreGetters = {
  getPlugins: () => PluginModel[];
  getIsLoading: () => boolean;
  getUnarchivedPlugins: () => PluginModel[];
  getPermissions: () => ResourceActions[];
};

type StoreActions = {
  refreshAuth: () => void;
  setPlugins: (plugins: PluginModel[]) => void;
  setLoadingPlugins: (status: boolean) => void;
  fetch: (projectID: number) => Promise<void>;
  fetchUnarchived: (projectID: number) => Promise<void>;
  setUnarchivedPlugins: (plugins: PluginModel[]) => void;
  add: (projectId: number, pluginCreate: CreatePluginModel) => Promise<number>;
  update: (
    projectId: number,
    pluginId: number,
    pluginUpdate: PluginEditModel,
  ) => Promise<PluginModel>;
  remove: (projectId: number, pluginId: number) => Promise<void>;
};

type Store = PiniaStore<'plugin', StoreState, StoreGetters, StoreActions>;

export const usePluginStore = (pinia: Pinia = piniaInstance): Store => {
  return useStore<Store, ApiStore<ProjectPluginsApi>>(
    'plugin',
    {
      state: {
        plugins: [],
        unarchivedPlugins: [],
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
        getPermissions(): ResourceActions[] {
          return this.permissions;
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
            const pluginGet: PluginListModel = await this.callApi(
              'projectsIdPluginsGet',
              {
                id,
              },
            );
            this.setPlugins(pluginGet.resources);
            this.setPermissions(pluginGet.permissions);
          } finally {
            this.setLoadingPlugins(false);
          }
        },
        async fetchUnarchived(projectID: number) {
          try {
            this.setLoadingPlugins(true);
            const pluginGet: PluginListModel = await this.callApi(
              'projectsIdUnarchivedPluginsGet',
              {
                id: projectID,
              },
            );
            this.setUnarchivedPlugins(pluginGet.resources);
            this.setPermissions(pluginGet.permissions);
          } finally {
            this.setLoadingPlugins(false);
          }
        },
        async add(projectId, pluginCreate): Promise<number> {
          try {
            this.setIsLoading(true);
            const response = await this.callApi('projectsProjectIdPluginsPut', {
              projectId: projectId,
              addProjectPluginRequest: pluginCreate,
            });
            this.fetch(projectId);
            this.fetchUnarchived(projectId);
            return response.id;
          } finally {
            this.setIsLoading(false);
          }
        },
        async update(projectId, pluginId, pluginUpdate) {
          try {
            this.setIsLoading(true);
            const response = await this.callApi(
              'projectsProjectIdPluginsPluginIdPatch',
              {
                projectId: projectId,
                pluginId: pluginId,
                updateProjectPluginRequest: pluginUpdate,
              },
            );
            this.fetch(projectId);
            this.fetchUnarchived(projectId);
            return response;
          } finally {
            this.setIsLoading(false);
          }
        },
        async remove(projectId, pluginId) {
          try {
            this.setIsLoading(true);
            await this.callApi('projectsProjectIdPluginsPluginIdDelete', {
              projectId: projectId,
              pluginId: pluginId,
            });
            this.fetch(projectId);
            this.fetchUnarchived(projectId);
          } finally {
            this.setIsLoading(false);
          }
        },
      },
    },
    useApiStore(ProjectPluginsApi, pinia),
  )(pinia);
};

type PluginStore = ReturnType<typeof usePluginStore>;
export type { PluginStore };
