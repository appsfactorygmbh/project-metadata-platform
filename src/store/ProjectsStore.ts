import { projectsService } from '../services/ProjectsService';
import type { ProjectModel } from '@/models/ProjectModel';
import { defineStore } from 'pinia';

export const ProjectsStore = defineStore('table', {
  state: () => {
    return {
      projects: [] as ProjectModel[],
      isLoading: false as boolean,
    };
  },
  getters: {
    getProjects(): ProjectModel[] {
      return this.projects;
    },
  },
  actions: {
    setProjects(projects: ProjectModel[]) {
      this.projects = projects;
    },
    setLoading(status: boolean) {
      this.isLoading = status;
    },

    async fetchProjects() {
      this.setLoading(true);
      const table: ProjectModel[] =
        (await projectsService.fetchProjects()) ?? [];
      this.setProjects(table);
      this.setLoading(false);
    },
  },
});
