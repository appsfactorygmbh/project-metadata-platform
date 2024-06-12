import {pluginService} from '../../services/Plugin/PluginService.ts';
import {defineStore} from 'pinia';

interface Plugin{
  PluginName: string
  Url: string
  DisplayName: string
}


export const PluginsStore = defineStore({
  id: "plugins",
  state: () => ({
    plugins: [] as Plugin[]
  }),
  getters: {
    getPlugins(): Plugin[] {
      return this.plugins
    }
  },
  actions: {
    setPlugins(plugins: Plugin[]): void {
      this.plugins = plugins
    },
    async fetchPlugins (projectID: string) {
      pluginService.fetchPlugins(projectID)
    },
  },
});

export type PluginsStoreType = Store<typeof PluginsStore>