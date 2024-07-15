import { defineStore } from 'pinia';
import type { PluginModel } from '@/models/Plugin';
import type { PluginEditModel } from '@/models/Plugin/PluginEditModel.ts';

type StoreState = {
  pluginChanges: Map<number, PluginEditModel>;
  projectInformationChanges: string[];
  canBeCreated: boolean;
  pluginsWithUrlConflicts: number[][];
  duplicatedUrls: Map<string, number[]>;
  emptyUrlFields: Map<number, number>;
  emptyDisplaynameFields: Map<number, number>;
};

export const useProjectEditStore = defineStore('projectEdit', {
  state: (): StoreState => {
    return {
      pluginChanges: new Map(),
      projectInformationChanges: [],
      canBeCreated: true,
      pluginsWithUrlConflicts: [],
      duplicatedUrls: new Map(),
      emptyUrlFields: new Map(),
      emptyDisplaynameFields: new Map(),
    };
  },

  getters: {
    // Returns all Plugins that are not deleted
    getPluginChanges(): PluginEditModel[] {
      return Array.from(this.pluginChanges.values()).filter(
        (plugin) => !plugin.isDeleted,
      );
    },
    // Return all Projectinformation changes (not implemented in this branch)
    getProjectInformationChanges(): string[] {
      return this.projectInformationChanges;
    },
    // Returns all Plugins that have URL conflicts (two or more Plugins have the same URL)
    getPluginsWithUrlConflicts(): number[] {
      return Array.from(this.duplicatedUrls.values())
        .filter((value) => value.length > 1)
        .flat();
    },
    // Returns whether the Project can be created (no URL conflicts and no empty fields)
    getCanBeAdded(): boolean {
      return (
        this.getPluginsWithUrlConflicts.length === 0 &&
        this.emptyUrlFields.size === 0 &&
        this.emptyDisplaynameFields.size === 0
      );
    },
  },

  actions: {
    // Adds an empty field to the emptyFields Map
    addEmptyUrlField(id: number): void {
      this.emptyUrlFields.set(id, id);
    },

    // Removes an empty field from the emptyFields Map
    removeEmptyUrlField(id: number): void {
      this.emptyUrlFields.delete(id);
    },

    addEmptyDisplaynameField(id: number): void {
      this.emptyDisplaynameFields.set(id, id);
    },

    // Removes an empty field from the emptyFields Map
    removeEmptyDisplaynameField(id: number): void {
      this.emptyDisplaynameFields.delete(id);
    },

    // Resets all changes made to the Plugins and Projectinformation
    resetChanges(): void {
      this.pluginChanges.clear();
      this.projectInformationChanges = [];
      this.canBeCreated = true;
      this.pluginsWithUrlConflicts = [];
      this.emptyUrlFields.clear();
      this.emptyDisplaynameFields.clear();
    },

    // Checks for URL conflicts between Plugins
    checkForConflicts(): void {
      this.pluginsWithUrlConflicts = [];
      this.duplicatedUrls = new Map();

      this.pluginChanges.forEach((plugin, key) => {
        if (this.duplicatedUrls.has(plugin.url) && plugin.isDeleted === false) {
          this.duplicatedUrls.get(plugin.url)?.push(key);
        } else {
          this.duplicatedUrls.set(plugin.url, [key]);
        }
      });
    },

    // Adds a Plugin to the pluginChanges Map (to sync with the Plugins in "ProjectPlugins")
    // Return an index to identify the Plugin when one Plugin wants to update or delete it
    initialAdd(plugin: PluginModel): number {
      const index = this.pluginChanges.size;
      const editPlugin: PluginEditModel = {
        ...plugin,
        editKey: index,
        isDeleted: false,
      };
      this.pluginChanges.set(index, editPlugin);
      return index;
    },

    // Updates the Plugin in the pluginChanges Map
    updatePluginChanges(id: number, plugin: PluginEditModel): void {
      this.pluginChanges.set(id, plugin);
    },

    // Sets isDeleted to true for the Plugin in the pluginChanges Map
    deletePlugin(id: number): void {
      const pluginChange = this.pluginChanges.get(id);
      if (pluginChange) {
        this.pluginChanges.set(id, { ...pluginChange, isDeleted: true });
      }
    },
  },
});

type ProjectEditStore = ReturnType<typeof useProjectEditStore>;
export type { ProjectEditStore };
