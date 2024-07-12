import { defineStore } from 'pinia';
import type { PluginModel } from '@/models/Plugin';

type StoreState = {
  pluginChanges: Map<string, PluginModel>;
  projectInformationChanges: [];
  deletedPlugin: PluginModel[];
  canBeCreated: boolean;
};

export const useProjectEditStore = defineStore('projectEdit', {
  state: (): StoreState => {
    return {
      pluginChanges: new Map(),
      projectInformationChanges: [],
      deletedPlugin: [],
      canBeCreated: true,
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
    getCanBeCreated(): boolean {
      return this.canBeCreated;
    },
  },

  actions: {
    resetChanges(): void {
      this.pluginChanges.clear();
      this.projectInformationChanges = [];
      this.deletedPlugin = [];
      this.canBeCreated = true;
    },

    setCanBeCreated(status: boolean): void {
      this.canBeCreated = status;
    },

    initialAdd(plugin: PluginModel): void {
      this.pluginChanges.set(plugin.id.toString() + plugin.url, plugin);
      console.log(this.pluginChanges);
    },

    updatePluginChanges(id: string, plugin: PluginModel): void {
      this.pluginChanges.set(id, plugin);
    },

    deletePlugin(id: number, url: string): void {
      this.pluginChanges.delete(id.toString() + url);
    },

    falseUrlInput(id: number, url: string): boolean {
      if (url === '' || url === undefined) {
        return true;
      }
      if (this.pluginChanges.has(id.toString() + url)) {
        if (this.pluginChanges.get(id.toString() + url)?.url === url) {
          return true;
        }
      }
      for (let i = 0; i < this.getPluginChanges.length; i++) {
        if (this.getPluginChanges[i]?.url === url) {
          return true;
        }
      }
      return false;
    },
  },
});

type ProjectEditStore = ReturnType<typeof useProjectEditStore>;
export type { ProjectEditStore };
