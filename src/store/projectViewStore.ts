import { projectsService } from '@/services/ProjectViewServices';
import type { Project } from '@/models/ProjectViewModel';
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
        clientName: '',
      };
      return this.projectView;
    },
  },
});
