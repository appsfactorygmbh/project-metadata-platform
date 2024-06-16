import { projectsService } from '../services/projectViewServices';
import { Project } from '../models/projectViewModel';
import { defineStore } from 'pinia';

//to store the project description
export const projectStore = defineStore({
  id: 'projectView',
  state: () => ({
    projectView: {} as Project,
  }),
  actions: {
    async getProjectView() {
      const project = await projectsService.fetchProject();
      this.projectView = project ?? {
        projectName: '',
        businessUnit: '',
        teamNumber: '',
        department: '',
        clientName: ''
      };
      return this.projectView;
    },
  },
});
