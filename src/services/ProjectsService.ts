import type { ProjectModel } from '@/models/ProjectModel';
import type { CreateProjectModel } from '@/models/CreateProjectModel.ts';

class ProjectsService {
  fetchProjects = async () => {
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL + '/projects',
      );

      const data: ProjectModel[] = await response.json();

      return data;
    } catch (err) {
      console.error('Error fetching projects: ' + err);
      return null;
    }
  };

  addProject = async (projectData: CreateProjectModel) => {
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL + '/projects',
        {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(projectData),
          mode: 'cors',
        },
      );
      return response;
    } catch (error) {
      console.error('Error:', error);
    }
  };
}

const projectsService = new ProjectsService();
export { projectsService };
export type { ProjectsService };
