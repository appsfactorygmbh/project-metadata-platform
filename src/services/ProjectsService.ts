import type {
  CreateProjectModel,
  DetailedProjectModel,
  ProjectModel,
  UpdateProjectModel,
} from '@/models/Project';
import { ApiService } from './ApiService';

class ProjectsService extends ApiService {
  fetchProjects = async (search?: string): Promise<ProjectModel[] | null> => {
    let url = `/Projects`;
    if (search) url += '?search=' + search;
    try {
      const response = await this.fetch(url);
      if (!response.ok) {
        throw new Error('Network response was not ok');
      }

      const data: ProjectModel[] = await response.json();

      return data;
    } catch (err) {
      console.error('Error fetching projects: ' + err);
      return null;
    }
  };

  fetchProject = async (id: number): Promise<DetailedProjectModel | null> => {
    try {
      const response = await this.fetch('/Projects/' + id.toString(), {
        headers: {
          Accept: 'text/plain',
          'Access-Control-Allow-Origin': '*',
          cors: 'cors',
        },
      });

      const data: DetailedProjectModel = await response.json();
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
      const response = await this.fetch('/projects', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(projectData),
        mode: 'cors',
      });
      return response;
    } catch (error) {
      console.error('Error:', error);
      return null;
    }
  };

  updateProject = async (
    projectData: UpdateProjectModel,
    id: number,
  ): Promise<Response | null> => {
    try {
      const response = await this.fetch('/Projects?projectId=' + id, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(projectData),
        mode: 'cors',
      });
      return response;
    } catch (error) {
      console.error('Failed to update Project via PUT Request: ', error);
      return null;
    }
  };

  archiveProject = async (
    projectData: UpdateProjectModel,
    id: number,
  ): Promise<Response | null> => {
    projectData.isArchived = true;
    try {
      const response = await this.updateProject(projectData, id);
      return response;
    } catch (error) {
      console.error('Failed to archive Project via PUT Request: ', error);
      return null;
    }
  };

  activateProject = async (
    projectData: UpdateProjectModel,
    id: number,
  ): Promise<Response | null> => {
    projectData.isArchived = false;
    try {
      const response = await this.updateProject(projectData, id);
      return response;
    } catch (error) {
      console.error('Failed to activate Project via PUT Request: ', error);
      return null;
    }
  };

  deleteProject = async (
    id: number
  ): Promise<Response | null> => {
    try {
      const response = await this.fetch(`/Projects/${id}`, {
        method: 'DELETE',
        headers: {
          Accept: 'application/json',
        },
      });
      return response;
    } catch (error) {
      console.error('Failed to delete project:', error);
      return null;
    }
  }
}

const projectsService = new ProjectsService();
export { projectsService };
export type { ProjectsService };
