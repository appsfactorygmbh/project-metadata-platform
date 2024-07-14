import { defineStore } from 'pinia';
import type { PluginModel } from '@/models/Plugin';
import type { PluginEditModel } from '@/models/Plugin/PluginEditModel.ts';

type StoreState = {
  pluginChanges: Map<number, PluginEditModel>;
  projectInformationChanges: [];
  canBeCreated: boolean;
  pluginsWithUrlConflicts: number[][];
};

export const useProjectEditStore = defineStore('projectEdit', {
  state: (): StoreState => {
    return {
      pluginChanges: new Map(),
      projectInformationChanges: [],
      canBeCreated: true,
      pluginsWithUrlConflicts: [],
    };
  },

  getters: {
    getPluginChanges(): PluginEditModel[] {
      return Array.from(this.pluginChanges.values()).filter(
        (plugin) => !plugin.isDeleted,
      );
    },
    getProjectInformationChanges(): [] {
      return this.projectInformationChanges;
    },
    getCanBeCreated(): boolean {
      return this.canBeCreated;
    },
    getPluginsWithUrlConflicts(): number[] {
      return this.pluginsWithUrlConflicts.map((conflict) => conflict[0]);
    },
    getCanBeAdded(): boolean {
      return this.canBeCreated && this.pluginsWithUrlConflicts.length === 0;
    },
  },

  actions: {
    resetChanges(): void {
      this.pluginChanges.clear();
      this.projectInformationChanges = [];
      this.canBeCreated = true;
      this.pluginsWithUrlConflicts = [];
    },

    checkForConflicts(id: number): void {
      for (let i = 0; i < this.getPluginChanges.length; i++) {
        // add a conflict if the urls are the same and the ids are different and the plugins are not deleted
        if (
          this.pluginChanges.get(id).url === this.pluginChanges.get(i).url &&
          id !== i &&
          !this.pluginChanges.get(id).isDeleted &&
          !this.pluginChanges.get(i).isDeleted
        ) {
          this.addConflict(id, i);
        }
      }
    },

    checkIfConflictsResolved(): void {
      for (let i = 0; i < this.pluginsWithUrlConflicts.length; i++) {
        if (
          this.pluginChanges.get(this.pluginsWithUrlConflicts[i][0]).url !==
            this.pluginChanges.get(this.pluginsWithUrlConflicts[i][1]).url ||
          this.pluginChanges.get(this.pluginsWithUrlConflicts[i][0])
            .isDeleted ||
          this.pluginChanges.get(this.pluginsWithUrlConflicts[i][1]).isDeleted
        ) {
          this.pluginsWithUrlConflicts.splice(i, 1);
        }
      }
    },

    addConflict(leftID: number, rightID: number): void {
      this.pluginsWithUrlConflicts.push([leftID, rightID]);
    },

    setCanBeCreated(status: boolean): void {
      this.canBeCreated = status;
    },

    initialAdd(plugin: PluginModel): number {
      console.log('plugin: ', plugin);
      const index = this.pluginChanges.size;
      const editPlugin: PluginEditModel = {
        ...plugin,
        editKey: index,
        isDeleted: false,
      };
      this.pluginChanges.set(index, editPlugin);
      console.log(this.pluginChanges);
      return index;
    },

    updatePluginChanges(id: number, plugin: PluginEditModel): void {
      this.pluginChanges.set(id, plugin);
      this.canBeCreated = true;
      this.checkForConflicts(id);
      this.checkIfConflictsResolved();
    },

    deletePlugin(id: number): void {
      this.pluginChanges.get(id).isDeleted = true;
      this.checkIfConflictsResolved()
    },
  },
});

type ProjectEditStore = ReturnType<typeof useProjectEditStore>;
export type { ProjectEditStore };
