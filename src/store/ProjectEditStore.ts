import { defineStore } from 'pinia';
import type { PluginModel } from '@/models/Plugin';

type StoreState = {
  pluginChanges: Map<string, PluginModel>;
  projectInformationChanges: [];
  deletedPlugin: PluginModel[];
  canBeCreated: boolean;
  gotEdited: boolean
};

export const useProjectEditStore = defineStore('projectEdit', {
  state: (): StoreState => {
    return {
      pluginChanges: new Map(),
      projectInformationChanges: [],
      deletedPlugin: [],
      canBeCreated: true,
      gotEdited: false
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
      this.canBeCreated = true;
      this.gotEdited = true
    },

    isCorrectUrlInput(id:string, input: PluginModel): boolean {
      if(input.url === "" || input.url == null) return false
      if(this.pluginChanges.has(input.id.toString() + input.url) && id !== input.id.toString() + input.url) return false
      for (let i = 0; i < this.getPluginChanges.length; i++) {
        if (this.getPluginChanges[i]?.url === input.url) return false;
      }
      return true;
    },

    deletePlugin(id: string): void {
      this.pluginChanges.delete(id);
    },
  },
});

type ProjectEditStore = ReturnType<typeof useProjectEditStore>;
export type { ProjectEditStore };
