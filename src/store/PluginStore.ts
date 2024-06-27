import { pluginService } from '@/services';
import { defineStore } from 'pinia';
import type { GlobalPluginModel, PluginModel } from '@/models/Plugin';

type StoreState = {
  plugins: PluginModel[];
  globalPlugins: GlobalPluginModel[];
  isLoadingPlugins: boolean;
  isLoadingGlobalPlugins: boolean;
  isLoadingDelete: boolean;
  removedSuccessfully: boolean;
};

export const usePluginsStore = defineStore('plugin', {
  state: (): StoreState => {
    return {
      plugins: [],
      globalPlugins: [],
      isLoadingPlugins: false,
      isLoadingGlobalPlugins: false,
      isLoadingDelete: false,
      removedSuccessfully: true,
    };
  },

  getters: {
    getPlugins(): PluginModel[] {
      return this.plugins;
    },
    getGlobalPlugins(): GlobalPluginModel[] {
      return this.globalPlugins;
    },
    getIsLoading(): boolean {
      return this.isLoadingPlugins || this.isLoadingPlugins;
    },
    getIsLoadingPlugins(): boolean {
      return this.isLoadingPlugins;
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
    setPlugins(plugins: PluginModel[]): void {
      this.plugins = plugins;
    },
    setGlobalPlugins(globalPlugins: GlobalPluginModel[]): void {
      this.globalPlugins = globalPlugins;
    },
    setLoadingPlugins(status: boolean): void {
      this.isLoadingPlugins = status;
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

    async fetchPlugins(projectID: number) {
      try {
        this.setLoadingPlugins(true);
        const plugins: PluginModel[] =
          await pluginService.fetchPlugins(projectID);
        this.setPlugins(plugins);
      } finally {
        this.setLoadingPlugins(false);
      }
    },

    async fetchGlobalPlugins() {
      try {
        this.setLoadingGlobalPlugins(true);
        const globalPlugins: GlobalPluginModel[] =
          await pluginService.fetchGlobalPlugins();
        this.setGlobalPlugins(globalPlugins);
      } finally {
        this.setLoadingGlobalPlugins(false);
      }
    },

    async deleteGlobalPlugin(pluginId: number) {
      try {
        this.setLoadingDelete(true);
        this.setRemovedSuccessfully(false);
        const response = await pluginService.removeGlobalPlugin(pluginId);
        if (response && response?.ok) this.setRemovedSuccessfully(true);
        else this.setRemovedSuccessfully(false);
      } finally {
        this.setLoadingDelete(false);
      }
    },
  },
});

type PluginsStore = ReturnType<typeof usePluginsStore>;
export type { PluginsStore };
