import { pluginService } from '@/services';
import { defineStore } from 'pinia';
import type { PluginModel } from '@/models/Plugin';

type StoreState = {
  plugins: PluginModel[];
  isLoading: boolean;
};

export const usePluginsStore = defineStore('plugin', {
  state: (): StoreState => {
    return {
      plugins: [],
      isLoading: false,
    };
  },

  getters: {
    getPlugins(): PluginModel[] {
      return this.plugins;
    },
    getIsLoading(): boolean {
      return this.isLoading;
    },
  },

  actions: {
    setPlugins(plugins: PluginModel[]): void {
      this.plugins = plugins;
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
  },
});
