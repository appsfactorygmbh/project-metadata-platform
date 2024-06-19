import { projectsService } from '@/services/ProjectInformationServices';
import type { Project } from '@/models/ProjectInformationModel';
import { defineStore } from 'pinia';

//to store the project description
export const projectStore = defineStore({
  id: 'ProjectInformation',
  state: () => ({
    ProjectInformation: {} as Project,
  }),
  actions: {
    async getProjectInformation() {
      const project = await projectsService.fetchProject();
      this.ProjectInformation = project ?? {
        id: 100,
        projectName: '',
        businessUnit: '',
        teamNumber: '',
        department: '',
        clientName: '',
      };
      return this.ProjectInformation;
    },
  },
});
