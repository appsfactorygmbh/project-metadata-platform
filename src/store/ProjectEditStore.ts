import { defineStore } from 'pinia';
import type {PluginModel} from "@/models/Plugin";

type StoreState = {
  pluginChanges: PluginModel[]
  projectInformationChanges: []
};

export const useProjectEditStore = defineStore('projectEdit', {
  state: (): StoreState => {
    return {
      pluginChanges: [],
      projectInformationChanges: []
    };
  },

  getters: {
    getPluginChanges(): PluginModel[] {
      return this.pluginChanges;
    },
    getProjectInformationChanges(): [] {
      return this.projectInformationChanges;
    },
  },

  actions: {
    resetChanges(): void {
      this.pluginChanges = [];
      this.projectInformationChanges = [];
    },
    updatePluginChanges(pluginID: number, url: string, newPlugin: PluginModel): void {
      console.log("im store")
      const pluginIndex = this.pluginChanges.findIndex(
        (plugin) => plugin.id === pluginID && plugin.url === url,
      );
      if(pluginIndex === -1) {
        this.pluginChanges.push(newPlugin);
      } else {
        this.pluginChanges[pluginIndex] = newPlugin;
      }
    },
    updateProjectInformationChanges(): void {
      console.log("update Project Information")
    }
  },
});

type ProjectEditStore = ReturnType<typeof useProjectEditStore>;
export type { ProjectEditStore };
