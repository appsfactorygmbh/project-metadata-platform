import { projectsService } from '@/services';
import type {
  CreateProjectModel,
  DetailedProjectModel,
  ProjectModel,
  UpdateProjectModel,
} from '@/models/Project';

import { ProjectsApi } from '@/api/generated';
import { type PiniaStore, useStore } from 'pinia-generic';
import { type ApiStore, apiStore } from './ApiStore';
import { piniaInstance } from './piniaInstance';

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

type StoreGetters = {
  getProjects: () => ProjectModel[];
  getProject: () => DetailedProjectModel | null;
  getIsLoading: () => boolean;
  getIsLoadingAdd: () => boolean;
  getIsLoadingUpdate: () => boolean;
  getIsLoadingProjects: () => boolean;
  getIsLoadingProject: () => boolean;
  getAddedSuccessfully: () => boolean;
  getUpdatedSuccessfully: () => boolean;
};

type StoreActions = {
  refreshAuth: () => void;
  fetchAll: (options?: {
    setCache?: boolean;
    search?: string;
  }) => Promise<ProjectModel[]>;
  fetch: (id: ProjectModel['id']) => Promise<DetailedProjectModel | null>;
  create: (project: CreateProjectModel) => Promise<void>;
  update: (
    project: UpdateProjectModel,
    id: ProjectModel['id'],
  ) => Promise<void>;
  setProjects: (projects: ProjectModel[]) => void;
  setProject: (project: DetailedProjectModel | null) => void;
  setLoadingAdd: (status: boolean) => void;
  setLoadingUpdate: (status: boolean) => void;
  setLoadingProjects: (status: boolean) => void;
  setLoadingProject: (status: boolean) => void;
  setAddedSuccessfully: (status: boolean) => void;
  setUpdatedSuccessfully: (status: boolean) => void;
  getSlugById: (id: number) => Promise<string>;
  getBySlug: (slug: string) => Promise<ProjectModel | null>;
};

type Store = PiniaStore<'project', StoreState, StoreGetters, StoreActions>;

export const projectStore = useStore<Store, ApiStore<ProjectsApi>>(
  'project',
  {
    state: {
      projects: [],
      isLoadingAdd: false,
      isLoadingUpdate: false,
      isLoadingProjects: false,
      isLoadingProject: false,
      addedSuccessfully: false,
      updatedSuccessfully: false,
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
      refreshAuth(): void {
        this.initApi();
      },

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

      async fetchAll({ setCache = true, search } = {}) {
        try {
          this.setLoadingProjects(true);
          const projects: ProjectModel[] =
            (await this.callApi('projectsGet', { search })) ?? [];
          if (setCache) this.setProjects(projects);
          return projects;
        } finally {
          this.setLoadingProjects(false);
        }
      },

      async fetch(id: ProjectModel['id']) {
        try {
          this.setLoadingProject(true);
          const project: DetailedProjectModel = await this.callApi(
            'projectsIdGet',
            {
              id,
            },
          );
          this.setProject(project);
          return project;
        } finally {
          this.setLoadingProject(false);
        }
      },

      async create(projectData: CreateProjectModel) {
        try {
          this.setLoadingAdd(true);
          this.setAddedSuccessfully(false);
          const response = await projectsService.create(projectData);
          if (response) {
            this.fetchAll();
            this.setAddedSuccessfully(true);
          } else this.setAddedSuccessfully(false);
        } finally {
          this.setLoadingAdd(false);
        }
      },

      async update(projectData: UpdateProjectModel, id: number) {
        try {
          this.setLoadingUpdate(true);
          this.setUpdatedSuccessfully(false);
          const response = await projectsService.update(projectData, id);
          if (response) {
            this.setUpdatedSuccessfully(true);
            await this.fetch(id);
          } else {
            this.setUpdatedSuccessfully(false);
          }
        } finally {
          this.setLoadingUpdate(false);
        }
      },

      async getSlugById(id: number): Promise<string> {
        let targetProject: DetailedProjectModel | ProjectModel | null = null;
        targetProject =
          this.projects.find((project) => project.id === id) ?? null;
        if (targetProject) return generateSlug(targetProject.projectName);

        targetProject = await this.fetch(id);
        if (targetProject) return generateSlug(targetProject.projectName);

        return '';
      },

      async getBySlug(slug: string): Promise<ProjectModel | null> {
        let targetProject: ProjectModel | null = null;
        try {
          this.setLoadingProject(true);
          targetProject =
            this.projects.find(
              (project) => generateSlug(project.projectName) === slug,
            ) ?? null;
          if (targetProject) return targetProject;
          return null;
        } finally {
          this.setLoadingProject(false);
        }
      },
    },
  },
  apiStore(ProjectsApi),
)(piniaInstance);

type ProjectsStore = typeof projectStore;
export type { ProjectsStore };

const generateSlug = (name: string) => name.replace(/ /g, '-');
