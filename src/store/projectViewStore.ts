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
        projectName: 'Your Project Name',
        businessUnit: 'Business Unit',
        teamNumber: 'Team Number',
        department: 'Department',
        clientName: 'Client Name'
      };
      return this.projectView;
    },
  },
});
