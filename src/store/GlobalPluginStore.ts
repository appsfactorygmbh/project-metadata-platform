import { globalPluginService } from '@/services';
import { defineStore } from 'pinia';
import type { GlobalPluginModel } from '@/models/Plugin';

type StoreState = {
  globalPlugins: GlobalPluginModel[];
  isPatchingGlobalPlugin: boolean;
  patchingSuccessfully: boolean;
  isLoadingGlobalPlugins: boolean;
  isLoadingDelete: boolean;
  removedSuccessfully: boolean;
};

export const useGlobalPluginsStore = defineStore('globalPlugin', {
  state: (): StoreState => {
    return {
      globalPlugins: [],
      isPatchingGlobalPlugin: false,
      patchingSuccessfully: false,
      isLoadingGlobalPlugins: false,
      isLoadingDelete: false,
      removedSuccessfully: true,
    };
  },

  getters: {
    getGlobalPlugins(): GlobalPluginModel[] {
      return this.globalPlugins;
    },
    getGlobalPluginById:
      (state) =>
      (id: number): GlobalPluginModel | undefined => {
        return state.globalPlugins.find(
          (plugin: GlobalPluginModel) => plugin.id === id,
        );
      },

    getIsPatchingGlobalPlugin(): boolean {
      return this.isPatchingGlobalPlugin;
    },
    getPatchingSuccessfully(): boolean {
      return this.patchingSuccessfully;
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
    setIsPatchingGlobalPlugin(status: boolean): void {
      this.isPatchingGlobalPlugin = status;
    },
    setPatchingSuccessfully(status: boolean): void {
      this.patchingSuccessfully = status;
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

    async patchGlobalPlugin(plugin: GlobalPluginModel) {
      try {
        this.setIsPatchingGlobalPlugin(true);
        const response = await globalPluginService.patchGlobalPlugin(plugin);
        if (response && response.ok) {
          this.fetchGlobalPlugins();
          this.setPatchingSuccessfully(true);
        } else {
          this.setPatchingSuccessfully(false);
        }
      } catch (error) {
        console.error('Error in patchGlobalPlugin:', error);
        this.setPatchingSuccessfully(false);
      } finally {
        this.setIsPatchingGlobalPlugin(false);
      }
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

    async deleteGlobalPlugin(pluginId: number) {
      try {
        this.setLoadingDelete(true);
        this.setRemovedSuccessfully(false);
        const response = await globalPluginService.removeGlobalPlugin(pluginId);
        if (response && response?.ok) {
          this.setRemovedSuccessfully(true);
          this.fetchGlobalPlugins();
        } else this.setRemovedSuccessfully(false);
      } finally {
        this.setLoadingDelete(false);
      }
    },
  },
});

type GlobalPluginsStore = ReturnType<typeof useGlobalPluginsStore>;
export type { GlobalPluginsStore };
