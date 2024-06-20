import { projectsService } from '@/services';
import type {
  ProjectModel,
  ProjectDetailedModel,
  CreateProjectModel,
} from '@/models/Project';
import { defineStore } from 'pinia';

type StoreState = {
  projects: ProjectModel[];
  project: ProjectDetailedModel | null;
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
    getProject(): ProjectDetailedModel | null {
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
    setProject(project: ProjectDetailedModel | null) {
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
        const table: ProjectModel[] =
          (await projectsService.fetchProjects()) ?? [];
        this.setProjects(table);
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
        if (response && response?.ok) this.setAddedSuccessfully(true);
        else this.setAddedSuccessfully(false);
      } finally {
        this.setLoadingAdd(false);
      }
    },

    async fetchProject(id: number) {
      try {
        this.setLoadingProject(true);
        const project: ProjectDetailedModel =
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
  },
});
