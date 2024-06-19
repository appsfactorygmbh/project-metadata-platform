import { projectsService } from '@/services/ProjectInformationServices';
import type { ProjectInformationModel } from '@/models/ProjectInformationModel';
import { defineStore } from 'pinia';

//to store the project description
export const ProjectInformationStore = defineStore({
  id: 'ProjectInformation',
  state: () => ({
    projectInformation: {} as ProjectInformationModel,
    isLoading: false as boolean,
  }),
  getters: {
    getProjectInformation(): ProjectInformationModel {
      return this.projectInformation;
    },
  },
  actions: {
    setProjectInformation(projectInformation: ProjectInformationModel) {
      this.projectInformation = projectInformation;
    },
    setLoading(status: boolean) {
      this.isLoading = status;
    },
    async fetchProjectInformation(id: number) {
      this.setLoading(true);
      const projectInformation: ProjectInformationModel =
        (await projectsService.fetchProject(id)) ?? {
          id: 100,
          projectName: '',
          businessUnit: '',
          teamNumber: '',
          department: '',
          clientName: '',
        };
      this.setProjectInformation(projectInformation);
      this.setLoading(false);
    },
  },
});
