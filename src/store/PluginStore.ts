import { pluginService } from '@/services';
import { defineStore } from 'pinia';
import type { PluginModel } from '@/models/Plugin';

type StoreState = {
  plugins: PluginModel[];
  isLoadingPlugins: boolean;
  cachePlugins: PluginModel[];
};

export const usePluginsStore = defineStore('plugin', {
  state: (): StoreState => {
    return {
      plugins: [],
      cachePlugins: [],
      isLoadingPlugins: false,
    };
  },

  getters: {
    getPlugins(): PluginModel[] {
      return this.plugins;
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
    setLoadingPlugins(status: boolean): void {
      this.isLoadingPlugins = status;
    },

    async fetchPlugins(projectID: number) {
      try {
        this.setLoadingPlugins(true);
        const plugins: PluginModel[] =
          await pluginService.fetchPlugins(projectID);
        this.setPlugins(plugins);
        this.cachePlugins = plugins;
      } finally {
        this.setLoadingPlugins(false);
      }
      console.log(this.getPlugins);
    },
  },
});

type PluginsStore = ReturnType<typeof usePluginsStore>;
export type { PluginsStore };
