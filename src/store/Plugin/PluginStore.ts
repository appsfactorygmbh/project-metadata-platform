import { pluginService } from '../../services/Plugin/PluginService.ts';
import { defineStore } from 'pinia';

interface Plugin {
  pluginName: string;
  url: string;
  displayName: string;
}

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
    setLoadingStatus(status: boolean): void {
      this.isLoading = status;
    },
    async fetchPlugins(projectID: string) {
      this.setLoadingStatus(true);
      const pluginsFromServer: Plugin[] =
        await pluginService.fetchPlugins(projectID);
      console.log('habe gefetched ', pluginsFromServer);
      this.setPlugins(pluginsFromServer);
      console.log('from store:', this.plugins);
      this.setLoadingStatus(false);
    },
  },
});
