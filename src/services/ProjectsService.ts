import type {
  ProjectModel,
  ProjectDetailedModel,
  CreateProjectModel,
} from '@/models/Project';

class ProjectsService {
  fetchProjects = async (): Promise<ProjectModel[] | null> => {
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

  fetchProject = async (id: number): Promise<ProjectDetailedModel | null> => {
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL + '/Projects/' + id.toString(),
        //'src/components/ProjectInformation/test.json',
        {
          headers: {
            Accept: 'text/plain',
            'Access-Control-Allow-Origin': '*',
            cors: 'cors',
          },
        },
      );

      const data: ProjectDetailedModel = await response.json();
      return data;
    } catch (err) {
      console.error('Error fetching project: ' + err);
      return null;
    }
  };

  addProject = async (
    projectData: CreateProjectModel,
  ): Promise<Response | null> => {
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
      return null;
    }
  };
}

const projectsService = new ProjectsService();
export { projectsService };
export type { ProjectsService };
