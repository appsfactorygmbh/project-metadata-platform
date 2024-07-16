import { projectsService } from '@/services';
import type {
  ProjectModel,
  DetailedProjectModel,
  CreateProjectModel,
} from '@/models/Project';
import { defineStore } from 'pinia';

type StoreState = {
  projects: ProjectModel[];
  project: DetailedProjectModel | null;
  isLoadingAdd: boolean;
  isLoadingProjects: boolean;
  isLoadingProject: boolean;
  addedSuccessfully: boolean;
};

export const useProjectStore = defineStore('project', {
  state: (): StoreState => {
    return {
      projects: [],
      project: null,
      isLoadingAdd: false,
      isLoadingProjects: false,
      isLoadingProject: false,
      addedSuccessfully: false,
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
    getIsLoadingProjects(): boolean {
      return this.isLoadingProjects;
    },
    getIsLoadingProject(): boolean {
      return this.isLoadingProject;
    },
    getAddedSuccessfully(): boolean {
      return this.addedSuccessfully;
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
    setLoadingProjects(status: boolean) {
      this.isLoadingProjects = status;
    },
    setLoadingProject(status: boolean) {
      this.isLoadingProject = status;
    },
    setAddedSuccessfully(status: boolean) {
      this.addedSuccessfully = status;
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
        console.log(response);
        if (response && response?.ok) {
          this.fetchProjects();
          this.setAddedSuccessfully(true);
        } else this.setAddedSuccessfully(false);
      } finally {
        this.setLoadingAdd(false);
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
  },
});

type ProjectStore = ReturnType<typeof useProjectStore>;
export type { ProjectStore };
