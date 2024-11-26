import { pluginService } from '@/services';
import { defineStore } from 'pinia';
import type { PluginModel } from '@/models/Plugin';

type StoreState = {
  plugins: PluginModel[];
  isLoadingPlugins: boolean;
  cachePlugins: PluginModel[];
  changedPlugins: PluginModel[];
  unarchivedPlugins: PluginModel[];
};

export const usePluginsStore = defineStore('plugin', {
  state: (): StoreState => {
    return {
      plugins: [],
      cachePlugins: [],
      changedPlugins: [],
      unarchivedPlugins: [],
      isLoadingPlugins: false,
    };
  },

  getters: {
    getPlugins(): PluginModel[] {
      return this.plugins;
    },
    getIsLoading(): boolean {
      return this.isLoadingPlugins;
    },
    getCachePlugins(): PluginModel[] {
      return this.cachePlugins;
    },
    getUnarchivedPlugins(): PluginModel[] {
      return this.unarchivedPlugins;
    },
  },

  actions: {
    setPlugins(plugins: PluginModel[]): void {
      this.plugins = plugins;
    },
    setLoadingPlugins(status: boolean): void {
      this.isLoadingPlugins = status;
    },
    setCachePlugins(plugins: PluginModel[]): void {
      this.cachePlugins = plugins;
    },
    setUnarchivedPlugins(plugins: PluginModel[]): void {
      this.unarchivedPlugins = plugins;
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
    },

    async fetchUnarchivedPlugins(projectID: number) {
      try {
        this.setLoadingPlugins(true);
        const plugins: PluginModel[] =
          await pluginService.fetchUnarchivedPlugins(projectID);
        this.setUnarchivedPlugins(plugins);
      } finally {
        this.setLoadingPlugins(false);
      }
    },
  },
});

type PluginsStore = ReturnType<typeof usePluginsStore>;
export type { PluginsStore };
