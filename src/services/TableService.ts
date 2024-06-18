import type { Project } from '@/models/TableModel';

class ProjectsService {
  fetchProjects = async () => {
    try {
      const response = await fetch(
        //import.meta.env.VITE_BACKEND_URL + '/projects',
        './src/components/Table/test.json',
      );

      const data: Project[] = await response.json();
      console.log(data);

      return data;
    } catch (err) {
      console.error('Error fetching projects: ' + err);
      return null;
    }
  };
}

const projectsService = new ProjectsService();
export { projectsService };
export type { ProjectsService };
