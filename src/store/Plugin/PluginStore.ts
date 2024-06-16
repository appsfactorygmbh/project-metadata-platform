import { pluginService } from '../../services/Plugin/PluginService.ts';
import { defineStore } from 'pinia';
import { Plugin } from '../../models/Plugin.ts';

export const usePluginsStore = defineStore('plugin', {
  state: () => {
    return {
      plugins: [] as Plugin[],
      isLoading: false as boolean,
    };
  },

  getters: {
    getPlugins(): Plugin[] {
      return this.plugins;
    },
  },

  actions: {
    setPlugins(plugins: Plugin[]): void {
      this.plugins = plugins;
    },
    setLoading(status: boolean): void {
      this.isLoading = status;
    },

    async fetchPlugins(projectID: string) {
      this.setLoading(true);
      const plugins: Plugin[] =
        await pluginService.fetchPlugins(projectID);
      this.setPlugins(plugins);
      this.setLoading(false);
    },
  },
});