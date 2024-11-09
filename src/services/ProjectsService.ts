import type {
  CreateProjectModel,
  DetailedProjectModel,
  ProjectModel,
  UpdateProjectModel,
} from '@/models/Project';
import { ApiService } from './ApiService';
import type { ProjectsApi } from '@/api/generated';
class ProjectsService extends ApiService<ProjectsApi> {
  fetchProjects = async (
    search?: string,
  ): Promise<ProjectModel[] | undefined> => {
    const projects = await this.callApi('projectsGet', {
      search: search,
    });
    if (!projects) return [];
    return projects;
  };

  fetchProject = async (
    id: number,
  ): Promise<DetailedProjectModel | undefined> => {
    const project = await this.callApi('projectsIdGet', {
      id,
    });
    if (!project) return;
    return project;
  };

  addProject = async (projectData: CreateProjectModel) => {
    const response = await this.callApi('projectsPut', {
      createProjectRequest: projectData,
    });
    if (!response) return;
    return response;
  };

  updateProject = async (projectData: UpdateProjectModel, id: number) => {
    const response = await this.callApi('projectsPut', {
      createProjectRequest: projectData,
      id,
    });
    if (!response) return;
    return response;
  };
}

const projectsService = new ProjectsService('ProjectsService');
export { projectsService };
export type { ProjectsService };
