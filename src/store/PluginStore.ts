import { pluginService } from '@/services';
import { defineStore } from 'pinia';
import type { PluginModel, GlobalPluginModel } from '@/models/Plugin';

type StoreState = {
  plugins: PluginModel[];
  isLoadingPlugins: boolean;
  isLoadingGlobalPlugins: boolean;
  cachePlugins: PluginModel[];
  changedPlugins: PluginModel[];
  globalPlugins: GlobalPluginModel[];
};

export const usePluginsStore = defineStore('plugin', {
  state: (): StoreState => {
    return {
      plugins: [],
      cachePlugins: [],
      changedPlugins: [],
      isLoadingPlugins: false,
      isLoadingGlobalPlugins: false,
      globalPlugins: [],
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
      return this.isLoadingPlugins;
    },
    getCachePlugins(): PluginModel[] {
      return this.cachePlugins;
    },
  },

  actions: {
    setPlugins(plugins: PluginModel[]): void {
      this.plugins = plugins;
    },
    setGlobalPlugins(plugins: GlobalPluginModel[]): void {
      this.globalPlugins = plugins;
    },
    setLoadingPlugins(status: boolean): void {
      this.isLoadingPlugins = status;
    },
    setLoadingGlobalPlugins(status: boolean): void {
      this.isLoadingGlobalPlugins = status;
    },
    setCachePlugins(plugins: PluginModel[]): void {
      this.cachePlugins = plugins;
    },

    async fetchPlugins(projectID: number) {
      try {
        this.setLoadingPlugins(true);
        const plugins: PluginModel[] =
          await pluginService.fetchPlugins(projectID);
        this.setPlugins(plugins);
        this.setCachePlugins(plugins);
      } finally {
        this.setLoadingPlugins(false);
      }
      console.log(this.getPlugins);
    },
  },
});

type PluginsStore = ReturnType<typeof usePluginsStore>;
export type { PluginsStore };
