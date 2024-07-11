import { defineStore } from 'pinia';
import type { PluginModel } from '@/models/Plugin';

type StoreState = {
  pluginChanges: Map<string, PluginModel>;
  projectInformationChanges: [];
  deletedPlugin: PluginModel[];
};

export const useProjectEditStore = defineStore('projectEdit', {
  state: (): StoreState => {
    return {
      pluginChanges: new Map(),
      projectInformationChanges: [],
      deletedPlugin: [],
    };
  },

  getters: {
    getPluginChanges(): PluginModel[] {
      return Array.from(this.pluginChanges.values());
    },
    getProjectInformationChanges(): [] {
      return this.projectInformationChanges;
    },
    getDeletedPlugins(): PluginModel[] {
      return this.deletedPlugin;
    },
  },

  actions: {
    resetChanges(): void {
      this.pluginChanges = new Map<string, PluginModel>();
      this.projectInformationChanges = [];
      this.deletedPlugin = [];
    },

    initialAdd(plugin: PluginModel): void {
      this.pluginChanges.set(plugin.id.toString() + plugin.url, plugin);
      console.log(this.pluginChanges);
    },

    updatePluginChanges(id: string, plugin: PluginModel): void {
      this.pluginChanges.set(id, plugin);
    },

    addDeletedPlugin(plugin: PluginModel): void {
      this.deletedPlugin.push(plugin);
    },
  },
});

type ProjectEditStore = ReturnType<typeof useProjectEditStore>;
export type { ProjectEditStore };
