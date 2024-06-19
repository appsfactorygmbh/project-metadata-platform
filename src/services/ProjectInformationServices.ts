import type { ProjectInformationModel } from '@/models/ProjectInformationModel';

//Service to fetch data from backend
class ProjectsService {
  fetchProject = async (
    id: number,
  ): Promise<ProjectInformationModel | null> => {
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

      const data: ProjectInformationModel = await response.json();
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
