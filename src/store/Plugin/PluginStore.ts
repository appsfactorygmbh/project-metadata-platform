import { pluginService } from '../../services/Plugin/PluginService.ts';
import { defineStore } from 'pinia';
import type { PluginType } from '../../models/PluginType.ts';

export const usePluginsStore = defineStore('plugin', {
  state: () => {
    return {
      plugins: [] as PluginType[],
      isLoading: false as boolean,
    };
  },

  getters: {
    getPlugins(): PluginType[] {
      return this.plugins;
    },
  },

  actions: {
    setPlugins(plugins: PluginType[]): void {
      this.plugins = plugins;
    },
    setLoading(status: boolean): void {
      this.isLoading = status;
    },

    async fetchPlugins(projectID: string) {
      this.setLoading(true);
      const plugins: PluginType[] = await pluginService.fetchPlugins(projectID);
      this.setPlugins(plugins);
      this.setLoading(false);
    },
  },
});
