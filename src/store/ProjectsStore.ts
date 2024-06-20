import { projectsService } from '../services/ProjectsService';
import type { ProjectModel } from '@/models/ProjectModel';
import type { CreateProjectModel } from '@/models/CreateProjectModel.ts';
import { defineStore } from 'pinia';

export const ProjectsStore = defineStore('table', {
  state: () => {
    return {
      projects: [] as ProjectModel[],
      isLoading: false as boolean,
      addedSuccessfully: false as boolean,
      isAdding: false as boolean,
    };
  },
  getters: {
    getProjects(): ProjectModel[] {
      return this.projects;
    },
    getIsAdding(): boolean {
      return this.isAdding;
    },
    getAddedSuccessfully(): boolean {
      return this.addedSuccessfully;
    },
  },
  actions: {
    setProjects(projects: ProjectModel[]) {
      this.projects = projects;
    },
    setLoading(status: boolean) {
      this.isLoading = status;
    },
    setIsAdding(status: boolean) {
      this.isAdding = status;
    },
    setAddedSuccessfully(status: boolean) {
      this.addedSuccessfully = status;
    },

    async fetchProjects() {
      this.setLoading(true);
      const table: ProjectModel[] =
        (await projectsService.fetchProjects()) ?? [];
      this.setProjects(table);
      this.setLoading(false);
    },

    async addProject(projectData: CreateProjectModel) {
      this.setAddedSuccessfully(false);
      this.setIsAdding(true);
      const response: Response | undefined =
        await projectsService.addProject(projectData);
      console.log(response);
      if (response?.ok && response != null) {
        this.setAddedSuccessfully(true);
      } else {
        this.setAddedSuccessfully(false);
      }
      this.setIsAdding(false);
    },
  },
});
