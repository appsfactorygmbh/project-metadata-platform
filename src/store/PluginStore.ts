import { pluginService } from '@/services';
import { defineStore } from 'pinia';
import type { PluginModel, GlobalPluginModel } from '@/models/Plugin';

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
    setGlobalPlugins(plugins: GlobalPluginModel[]): void {
      this.globalPlugins = plugins;
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
