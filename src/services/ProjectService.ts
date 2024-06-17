import type { ProjectType } from '@/models/TableModel';
import type { CreateProjectType } from '@/models/CreateProjectModel';

class ProjectsService {
  fetchProjects = async () => {
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL + '/projects',
      );

      const data: ProjectType[] = await response.json();

      return data;
    } catch (err) {
      console.error('Error fetching projects: ' + err);
    }
  };

  addProject = async (projectData: CreateProjectType) => {
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL + '/projects',
        {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(projectData),
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
