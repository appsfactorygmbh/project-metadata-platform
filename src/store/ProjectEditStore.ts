import { defineStore } from 'pinia';
import type { PluginModel } from '@/models/Plugin';
import type { PluginEditModel } from '@/models/Plugin/PluginEditModel.ts';

type StoreState = {
  pluginChanges: Map<number, PluginEditModel>;
  projectInformationChanges: [];
  canBeCreated: boolean;
  pluginsWithUrlConflicts: Map<number, number>;
};

export const useProjectEditStore = defineStore('projectEdit', {
  state: (): StoreState => {
    return {
      pluginChanges: new Map(),
      projectInformationChanges: [],
      canBeCreated: true,
      pluginsWithUrlConflicts: new Map(),
    };
  },

  getters: {
    getPluginChanges(): PluginEditModel[] {
      return Array.from(this.pluginChanges.values()).filter(plugin => !plugin.isDeleted);
    },
    getProjectInformationChanges(): [] {
      return this.projectInformationChanges;
    },
    getCanBeCreated(): boolean {
      return this.canBeCreated;
    },
    getPluginsWithUrlConflicts(): Map<number, number> {
      // return Array.from(this.pluginsWithUrlConflicts.values());
      return this.pluginsWithUrlConflicts;
    }
  },

  actions: {
    resetChanges(): void {
      this.pluginChanges.clear();
      this.projectInformationChanges = [];
      this.canBeCreated = true;
    },

    setCanBeCreated(status: boolean): void {
      this.canBeCreated = status;
    },

    initialAdd(plugin: PluginModel): number {
      console.log("plugin: ", plugin)
      const index = this.pluginChanges.size;
      const editPlugin: PluginEditModel = {
        ...plugin,
        editKey: index,
        isDeleted: false,
        // hasConflicts: false,
      };
      this.pluginChanges.set(index, editPlugin);
      console.log(this.pluginChanges)
      return index
    },

    updatePluginChanges(id: number, plugin: PluginModel): void {
      this.pluginChanges.set(id, plugin);
      this.canBeCreated = true;
    },

    addConflictPlugin(id: number){
      this.pluginsWithUrlConflicts.set(id, id)
      console.log("neuer konflikt")
    },

    deleteConflictPlugin(id: number) {
      this.pluginsWithUrlConflicts.delete(id)
    },

    isCorrectUrlInput(id: number, plugin: PluginModel): boolean {
      for(let i = 0; i < this.getPluginChanges.length; i++) {
        if(this.getPluginChanges[i].url === plugin.url && this.pluginChanges.get(i).id !== id && !this.getPluginChanges[i].isDeleted){
            console.log("problem bei url")
            this.addConflictPlugin(id)
            return false
        }
      }
      this.deleteConflictPlugin(id)
      return true
    },

    recheckConflicts(): void {
      const plugins = this.getPluginChanges
      console.log("pluginChanges: ",plugins)
      for (let i = 0; i < plugins.length; i++){
        this.isCorrectUrlInput(this.getPluginChanges[i].editKey, this.getPluginChanges[i])
      }
    },

    deletePlugin(id: number): void {
      const plugin = this.pluginChanges.get(id);
      if (plugin) {
        this.pluginChanges.set(id, { ...plugin, isDeleted: true });
      }
      this.recheckConflicts()
    },
  },
});

type ProjectEditStore = ReturnType<typeof useProjectEditStore>;
export type { ProjectEditStore };
