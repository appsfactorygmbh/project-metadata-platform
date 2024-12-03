import { projectsService } from '@/services';
import type {
  CreateProjectModel,
  DetailedProjectModel,
  ProjectModel,
  UpdateProjectModel,
} from '@/models/Project';

import { defineStore } from 'pinia';

type StoreState = {
  projects: ProjectModel[];
  project: DetailedProjectModel | null;
  isLoadingAdd: boolean;
  isLoadingUpdate: boolean;
  isLoadingProjects: boolean;
  isLoadingProject: boolean;
  addedSuccessfully: boolean;
  updatedSuccessfully: boolean;
};

export const useProjectStore = defineStore('project', {
  state: (): StoreState => {
    return {
      projects: [],
      project: null,
      isLoadingAdd: false,
      isLoadingUpdate: false,
      isLoadingProjects: false,
      isLoadingProject: false,
      addedSuccessfully: false,
      updatedSuccessfully: false,
    };
  },
  getters: {
    getProjects(): ProjectModel[] {
      return this.projects;
    },
    getProject(): DetailedProjectModel | null {
      return this.project;
    },
    getIsLoading(): boolean {
      return (
        this.isLoadingAdd || this.isLoadingProjects || this.isLoadingProject
      );
    },
    getIsLoadingAdd(): boolean {
      return this.isLoadingAdd;
    },
    getIsLoadingUpdate(): boolean {
      return this.isLoadingUpdate;
    },
    getIsLoadingProjects(): boolean {
      return this.isLoadingProjects;
    },
    getIsLoadingProject(): boolean {
      return this.isLoadingProject;
    },
    getAddedSuccessfully(): boolean {
      return this.addedSuccessfully;
    },
    getUpdatedSuccessfully(): boolean {
      return this.updatedSuccessfully;
    },
  },
  actions: {
    setProjects(projects: ProjectModel[]) {
      this.projects = projects;
    },
    setProject(project: DetailedProjectModel | null) {
      this.project = project;
    },
    setLoadingAdd(status: boolean) {
      this.isLoadingAdd = status;
    },
    setLoadingUpdate(status: boolean) {
      this.isLoadingUpdate = status;
    },
    setLoadingProjects(status: boolean) {
      this.isLoadingProjects = status;
    },
    setLoadingProject(status: boolean) {
      this.isLoadingProject = status;
    },
    setAddedSuccessfully(status: boolean) {
      this.addedSuccessfully = status;
    },
    setUpdatedSuccessfully(status: boolean) {
      this.updatedSuccessfully = status;
    },

    async fetchProjects() {
      try {
        this.setLoadingProjects(true);
        const projects: ProjectModel[] =
          (await projectsService.fetchProjects()) ?? [];
        this.setProjects(projects);
      } finally {
        this.setLoadingProjects(false);
      }
    },

    async addProject(projectData: CreateProjectModel) {
      try {
        this.setLoadingAdd(true);
        this.setAddedSuccessfully(false);
        const response = await projectsService.addProject(projectData);
        if (response && response?.ok) {
          this.fetchProjects();
          this.setAddedSuccessfully(true);
        } else this.setAddedSuccessfully(false);
      } finally {
        this.setLoadingAdd(false);
      }
    },

    async updateProject(projectData: UpdateProjectModel, id: number) {
      try {
        this.setLoadingUpdate(true);
        this.setUpdatedSuccessfully(false);
        const response = await projectsService.updateProject(projectData, id);
        if (response && response?.ok) {
          this.setUpdatedSuccessfully(true);
          await this.fetchProject(id);
        } else {
          this.setUpdatedSuccessfully(false);
        }
      } finally {
        this.setLoadingUpdate(false);
      }
    },

    async fetchProject(id: number) {
      try {
        this.setLoadingProject(true);
        const project: DetailedProjectModel =
          (await projectsService.fetchProject(id)) ?? {
            id: 0,
            projectName: '',
            businessUnit: '',
            teamNumber: 0,
            department: '',
            clientName: '',
            isArchived: false,
          };
        this.setProject(project);
      } finally {
        this.setLoadingProject(false);
      }
    },

    async getProjectSlagById(id: number): Promise<string> {
      let targetProject: DetailedProjectModel | ProjectModel | null = null;
      targetProject =
        this.projects.find((project) => project.id === id) ?? null;
      if (targetProject) return targetProject.projectName.replace(/ /g, '-');

      targetProject = await projectsService.fetchProject(id);
      if (targetProject) return targetProject.projectName.replace(/ /g, '-');

      return '';
    },

    async getProjectBySlag(slag: string): Promise<ProjectModel | null> {
      let targetProject: ProjectModel | null = null;
      try {
        this.setLoadingProject(true);
        targetProject =
          this.projects.find(
            (project) => project.projectName.replace(/ /g, '-') === slag,
          ) ?? null;
        if (targetProject) return targetProject;
        return null;
      } finally {
        this.setLoadingProject(false);
      }
    },

    async archiveProject(projectData: UpdateProjectModel, id: number) {
      try {
        this.setLoadingUpdate(true);
        this.setUpdatedSuccessfully(false);
        const response = await projectsService.archiveProject(projectData, id);
        if (response && response?.ok) {
          this.setUpdatedSuccessfully(true);
          await this.fetchProject(id);
        } else {
          this.setUpdatedSuccessfully(false);
        }
      } finally {
        this.setLoadingUpdate(false);
      }
    },

    async activateProject(projectData: UpdateProjectModel, id: number) {
      try {
        this.setLoadingUpdate(true);
        this.setUpdatedSuccessfully(false);
        const response = await projectsService.activateProject(projectData, id);
        if (response && response?.ok) {
          this.setUpdatedSuccessfully(true);
          await this.fetchProject(id);
        } else {
          this.setUpdatedSuccessfully(false);
        }
      } finally {
        this.setLoadingUpdate(false);
      }
    },
  },
});

type ProjectStore = ReturnType<typeof useProjectStore>;
export type { ProjectStore };
