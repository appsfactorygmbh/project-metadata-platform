import { pluginService } from '@/services';
import { defineStore } from 'pinia';
import type { GlobalPluginModel, PluginModel } from '@/models/Plugin';

type StoreState = {
  plugins: PluginModel[];
  globalPlugins: GlobalPluginModel[];
  isLoading: boolean;
};

export const usePluginsStore = defineStore('plugin', {
  state: (): StoreState => {
    return {
      plugins: [],
      globalPlugins: [],
      isLoading: false,
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
      return this.isLoading;
    },
  },

  actions: {
    setPlugins(plugins: PluginModel[]): void {
      this.plugins = plugins;
    },
    setGlobalPlugins(globalPlugins: GlobalPluginModel[]): void {
      this.globalPlugins = globalPlugins;
    },
    setLoading(status: boolean): void {
      this.isLoading = status;
    },

    async fetchPlugins(projectID: number) {
      try {
        this.setLoading(true);
        const plugins: PluginModel[] =
          await pluginService.fetchPlugins(projectID);
        this.setPlugins(plugins);
      } finally {
        this.setLoading(false);
      }
    },
    async fetchGlobalPlugins() {
      try {
        this.setLoading(true);
        const globalPlugins: GlobalPluginModel[] =
          await pluginService.fetchGlobalPlugins();
        this.setGlobalPlugins(globalPlugins);
      } finally {
        this.setLoading(false);
      }
    },
  },
});

type PluginsStore = ReturnType<typeof usePluginsStore>;
export type { PluginsStore };
