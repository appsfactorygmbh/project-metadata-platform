import { defineStore } from 'pinia';
import type { DetailedProjectModel } from '@/models/Project';
import type { EditProjectModel } from '@/models/Project/EditProjectModel';

type StoreState = {
  projectInformationChanges: EditProjectModel;
  canBeCreated: boolean;
  duplicatedUrls: Map<string, number[]>;
  emptyUrlFields: Map<number, number>;
  emptyDisplaynameFields: Map<number, number>;
  emptyProjectInformationFields: Map<string, number>;
};

export const useProjectEditStore = defineStore('projectEdit', {
  state: (): StoreState => {
    return {
      canBeCreated: true,
      duplicatedUrls: new Map(),
      emptyUrlFields: new Map(),
      emptyDisplaynameFields: new Map(),
      emptyProjectInformationFields: new Map(),
      projectInformationChanges: {
        projectName: '',
        clientName: '',
        teamId: null,
        companyId: 0,
        ismsLevel: 'NORMAL',
        isEoC: false,
        companyState: 'EXTERNAL',
        notes: '',
      },
    };
  },

  getters: {
    // Return all Projectinformation changes (not implemented in this branch)
    getProjectInformationChanges(): EditProjectModel {
      return this.projectInformationChanges;
    },

    // Returns whether the Project can be created (no URL conflicts and no empty fields)
    getCanBeAdded(): boolean {
      return (
        this.emptyUrlFields.size === 0 &&
        this.emptyDisplaynameFields.size === 0 &&
        this.projectInformationChanges.projectName !== '' &&
        this.projectInformationChanges.clientName !== '' &&
        this.emptyProjectInformationFields.size === 0
      );
    },
  },

  actions: {
    // Adds an empty field to the emptyFields Map
    addEmptyUrlField(id: number): void {
      this.emptyUrlFields.set(id, id);
    },
    // Adds an empty field to the emptyFields Map
    addEmptyProjectInformationField(prop: string): void {
      this.emptyProjectInformationFields.set(prop, 1);
    },
    // Removes an empty field from the emptyFields Map
    removeEmptyProjectInformationField(prop: string): void {
      if (this.emptyProjectInformationFields.has(prop)) {
        this.emptyProjectInformationFields.delete(prop);
      }
    },
    // Sets the Projectinformation changes
    setProjectInformation(project: DetailedProjectModel): void {
      this.emptyProjectInformationFields.clear();
      const projectChanges: EditProjectModel = {
        ...project,
        companyId: project.company.id,
        teamId: project.team?.id,
      };
      this.projectInformationChanges = projectChanges;
    },

    // Updates the Projectinformation changes
    updateProjectInformationChanges(project: EditProjectModel): void {
      this.projectInformationChanges = project;
    },

    // Removes an empty field from the emptyFields Map
    removeEmptyUrlField(id: number): void {
      this.emptyUrlFields.delete(id);
    },
    // Adds an empty field to the emptyFields Map
    addEmptyDisplaynameField(id: number): void {
      this.emptyDisplaynameFields.set(id, id);
    },

    // Removes an empty field from the emptyFields Map
    removeEmptyDisplaynameField(id: number): void {
      this.emptyDisplaynameFields.delete(id);
    },

    // Resets all changes made to the Plugins and Projectinformation
    resetPluginChanges(): void {
      this.canBeCreated = true;
      this.emptyUrlFields.clear();
      this.emptyDisplaynameFields.clear();
    },
  },
});

type ProjectEditStore = ReturnType<typeof useProjectEditStore>;
export type { ProjectEditStore };
