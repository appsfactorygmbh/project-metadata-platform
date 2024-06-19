import type { Project } from '@/models/ProjectInformationModel';

//Service to fetch data from backend
class ProjectsService {
  fetchProjectID = async (): Promise<number | null> => {
    try {
      const data = 100;
      return data;
    } catch (err) {
      console.error('Error fetching project ID: ' + err);
      return null;
    }
  };

  fetchProject = async (): Promise<Project | null> => {
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL +
          '/Projects/' +
          (await this.fetchProjectID()),
        //'src/components/ProjectInformation/test.json',
        {
          headers: {
            Accept: 'text/plain',
            'Access-Control-Allow-Origin': '*',
            cors: 'cors',
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
