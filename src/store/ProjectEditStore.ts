import { defineStore } from 'pinia';
import type { PluginModel } from '@/models/Plugin';
import type { PluginEditModel } from '@/models/Plugin/PluginEditModel.ts';

type StoreState = {
  pluginChanges: Map<number, PluginEditModel>;
  projectInformationChanges: [];
  canBeCreated: boolean;
  pluginsWithUrlConflicts: Map<number, number[]>;
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
    getPluginsWithUrlConflicts(): number[] {
      // return Array.from(this.pluginsWithUrlConflicts.values());
      console.log(Array.from(this.pluginsWithUrlConflicts.keys()))
      return Array.from(this.pluginsWithUrlConflicts.keys());
    },
    getCanBeAdded(): boolean {
      return this.canBeCreated && this.pluginsWithUrlConflicts.size === 0;
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
      this.isCorrectUrlInput(id, plugin)
    },

    addConflictPlugin(currentID: number, conflictID: number): void {
      if(this.pluginsWithUrlConflicts.has(currentID)){
        const value = this.pluginsWithUrlConflicts.get(currentID)
        if(value){
          value.push(conflictID)
          this.pluginsWithUrlConflicts.set(currentID, value)
        }
      }
      this.pluginsWithUrlConflicts.set(currentID, [conflictID])
    },

    deleteConflictPlugin(id: number) {
      this.pluginsWithUrlConflicts.delete(id)
    },

    isCorrectUrlInput(id: number, plugin: PluginModel): boolean {
      if(plugin.url === ""){
        this.addConflictPlugin(id, id)
        return false
      }
      let notFound = true
      this.pluginChanges.forEach((value, key) => {
        if(value.url === plugin.url && key !== id && !value.isDeleted){
          this.addConflictPlugin(id, key)
          console.log("conflict map: ", this.pluginsWithUrlConflicts)
          notFound = false
          return false
        }
      });
      console.log("kein konflikt")
      if(notFound){
        this.deleteConflictPlugin(id)
      }
      // this.deleteConflictPlugin(id)
      this.resolvePotentialConflicts(id)
      return true
    },

    resolvePotentialConflicts(id: number): void {
      this.pluginsWithUrlConflicts.forEach((value, key) => {
        console.log('Checking id:', id, 'with value:', value);
        if(value.includes(id)){
          console.log("found conflict that can be resolved");
          const index = value.indexOf(id);
          if(index > -1){
            value.splice(index, 1);
          }
          if(value.length === 0){
            this.pluginsWithUrlConflicts.delete(key);
            console.log("deleted because no conflicts");
          }
        }
      });
    },

    deletePlugin(id: number): void {
      const plugin = this.pluginChanges.get(id);
      if (plugin) {
        this.pluginChanges.set(id, { ...plugin, isDeleted: true });
      }
      this.resolvePotentialConflicts(id)
    },
  },
});

type ProjectEditStore = ReturnType<typeof useProjectEditStore>;
export type { ProjectEditStore };
