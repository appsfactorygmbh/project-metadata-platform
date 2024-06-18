import type { Project } from '@/models/ProjectViewModel';

//Service to fetch data from backend
class ProjectsService {
  fetchProject = async (): Promise<Project | null> => {
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL + '/Projects',
        {
          headers: {
            Accept: 'text/plain',
            'Access-Control-Allow-Origin': '*',
            cors: 'no-cors',
          },
        },
      );

      // Fetch from local for test
      //const response = await fetch('src/components/projectView/test.json');
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
