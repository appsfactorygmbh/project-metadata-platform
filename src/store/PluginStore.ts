import { pluginService } from '@/services';
import { defineStore } from 'pinia';
import type { PluginModel, GlobalPluginModel } from '@/models/Plugin';

type StoreState = {
  plugins: PluginModel[];
  isLoadingPlugins: boolean;
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

    async fetchGlobalPlugins() {
      try {
        this.setLoading(true);
        const plugins: GlobalPluginModel[] =
          await pluginService.fetchGlobalPlugins();
        this.setGlobalPlugins(plugins);
      } finally {
        this.setLoading(false);
      }
    },

    async createPlugin(plugin: PluginModel) {
      try {
        this.setLoading(true);
        await pluginService.createPlugin(plugin);
      } finally {
        this.setLoading(false);
      }
    },
  },
});

type PluginsStore = ReturnType<typeof usePluginsStore>;
export type { PluginsStore };
