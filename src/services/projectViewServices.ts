import type { Project } from '@/models/projectViewModel';

//Service to fetch data from backend
class ProjectsService {
  fetchProject = async (): Promise<Project | null> => {
    try {
      const response = await fetch(
        //import.meta.env.VITE_BACKEND_URL + '/Projects',
        'src/components/projectView/test.json',
        {
          headers: {
            Accept: 'text/plain',
            'Access-Control-Allow-Origin': '*',
            cors: 'no-cors',
          },
        },
      );

      const data: Project = await response.json();
      return data;
    } catch (err) {
      console.error('Error fetching project: ' + err);
      return null;
    }
  };
}

const projectsService = new ProjectsService();
export { projectsService };
export type { ProjectsService };
