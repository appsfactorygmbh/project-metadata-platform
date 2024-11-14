import { globalPluginService } from '@/services';
import { defineStore } from 'pinia';
import type { GlobalPluginModel } from '@/models/Plugin';

type StoreState = {
  globalPlugins: GlobalPluginModel[];
  isLoadingGlobalPlugins: boolean;
  isLoadingDelete: boolean;
  removedSuccessfully: boolean;
};

export const useGlobalPluginsStore = defineStore('globalPlugin', {
  state: (): StoreState => {
    return {
      globalPlugins: [],
      isLoadingGlobalPlugins: false,
      isLoadingDelete: false,
      removedSuccessfully: true,
    };
  },

  getters: {
    getGlobalPlugins(): GlobalPluginModel[] {
      return this.globalPlugins;
    },
    getIsLoadingGlobalPlugins(): boolean {
      return this.isLoadingGlobalPlugins;
    },
    getIsLoadingDelete(): boolean {
      return this.isLoadingDelete;
    },
    getRemovedSuccessfully(): boolean {
      return this.removedSuccessfully;
    },
  },

  actions: {
    setGlobalPlugins(globalPlugins: GlobalPluginModel[]): void {
      this.globalPlugins = globalPlugins;
    },
    setLoadingGlobalPlugins(status: boolean): void {
      this.isLoadingGlobalPlugins = status;
    },
    setLoadingDelete(status: boolean): void {
      this.isLoadingDelete = status;
    },
    setRemovedSuccessfully(status: boolean): void {
      this.removedSuccessfully = status;
    },

    async fetchGlobalPlugins() {
      try {
        this.setLoadingGlobalPlugins(true);
        const globalPlugins: GlobalPluginModel[] =
          await globalPluginService.fetchGlobalPlugins();
        this.setGlobalPlugins(globalPlugins);
      } finally {
        this.setLoadingGlobalPlugins(false);
      }
    },

    async fetchGlobalPlugin(pluginId: number) {
      if (this.globalPlugins.length === 0) {
        await this.fetchGlobalPlugins();
      }
      return this.globalPlugins.find((plugin) => plugin.id === pluginId);
    },

    async archiveGlobalPlugin(pluginId: number) {
      console.log('deleting', pluginId);
      try {
        this.setLoadingDelete(true);
        this.setRemovedSuccessfully(false);
        const response =
          await globalPluginService.archiveGlobalPlugin(pluginId);
        if (response && response?.ok) {
          this.setRemovedSuccessfully(true);
          this.fetchGlobalPlugins();
        } else this.setRemovedSuccessfully(false);
      } finally {
        this.setLoadingDelete(false);
      }
    },

    async createGlobalPlugin(plugin: Omit<GlobalPluginModel, 'id'>) {
      try {
        const response = await globalPluginService.createGlobalPlugin(plugin);
        if (response && response.ok) {
          this.fetchGlobalPlugins();
        }
      } catch (err) {
        console.error('Error creating global plugin: ' + err);
      }
    },

    async updateGlobalPlugin(plugin: GlobalPluginModel) {
      try {
        const response = await globalPluginService.updateGlobalPlugin(plugin);
        if (response && response.ok) {
          this.fetchGlobalPlugins();
        } else {
          throw new Error('Failed to update global plugin');
        }
      } catch (err) {
        console.error('Error updating global plugin:', err);
        throw err;
      }
    },
  },
});

type GlobalPluginsStore = ReturnType<typeof useGlobalPluginsStore>;
export type { GlobalPluginsStore };
