import { pluginService } from '@/services';
import { defineStore } from 'pinia';
import type { PluginModel } from '@/models/Plugin';

type StoreState = {
  plugins: PluginModel[];
  isLoadingPlugins: boolean;
  cachePlugins: PluginModel[];
  changedPlugins: PluginModel[];
};

export const usePluginsStore = defineStore('plugin', {
  state: (): StoreState => {
    return {
      plugins: [],
      cachePlugins: [],
      changedPlugins: [],
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
    setCachePlugins(plugins: PluginModel[]): void {
      this.cachePlugins = plugins;
    },
    updatePluginURL(
      pluginID: number,
      oldPluginURL: string,
      newPluginURL: string,
    ): void {
      const pluginIndex = this.cachePlugins.findIndex(
        (plugin) => plugin.id === pluginID && plugin.url === oldPluginURL,
      );
      this.cachePlugins[pluginIndex].url = newPluginURL;
    },

    updateDisplayName(
      pluginID: number,
      pluginUrl: string,
      newDisplayName: string,
    ): void {
      const pluginIndex = this.cachePlugins.findIndex(
        (plugin) => plugin.id === pluginID && plugin.url === pluginUrl,
      );
      this.cachePlugins[pluginIndex].displayName = newDisplayName;
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
