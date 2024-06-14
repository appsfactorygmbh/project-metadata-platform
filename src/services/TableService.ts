import { Project } from 'models/TableModel';

class ProjectsService {
  fetchProjects = async () => {
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL + '/projects',
      );

      const data: Project[] = await response.json();

      return data;
    } catch (err) {
      console.error('Error fetching projects: ' + err);
    }
  };
}

const projectsService = new ProjectsService();
export { projectsService };
