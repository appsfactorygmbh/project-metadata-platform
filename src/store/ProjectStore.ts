import type {
  CreateProjectModel,
  DetailedProjectModel,
  ProjectModel,
  UpdateProjectModel,
} from '@/models/Project';

import { ProjectsApi } from '@/api/generated';
import { type PiniaStore, useStore } from 'pinia-generic';
import { type ApiStore, useApiStore } from './ApiStore';
import { piniaInstance } from './piniaInstance';
import type { Pinia } from 'pinia';

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
  fetchBySlug: (
    slug: ProjectModel['slug'],
  ) => Promise<DetailedProjectModel | null>;
  create: (project: CreateProjectModel) => Promise<void>;
  update: (
    id: ProjectModel['id'],
    project: UpdateProjectModel,
  ) => Promise<DetailedProjectModel | null>;
  archive: (id: ProjectModel['id']) => Promise<void>;
  unarchive: (id: ProjectModel['id']) => Promise<void>;
  delete: (id: ProjectModel['id']) => Promise<void>;
  findProjectById: <Full extends boolean>(
    id: ProjectModel['id'],
    { fullObjectNeeded }: { fullObjectNeeded?: Full },
  ) => Promise<
    (Full extends true ? DetailedProjectModel : ProjectModel) | null
  >;
  findProjectBySlug: <Full extends boolean>(
    slug: ProjectModel['slug'],
    { fullObjectNeeded }: { fullObjectNeeded?: Full },
  ) => Promise<
    (Full extends true ? DetailedProjectModel : ProjectModel) | null
  >;
  setProjects: (projects: ProjectModel[]) => void;
  setProject: (project: DetailedProjectModel | null) => void;
  setLoadingAdd: (status: boolean) => void;
  setLoadingUpdate: (status: boolean) => void;
  setLoadingProjects: (status: boolean) => void;
  setLoadingProject: (status: boolean) => void;
  setAddedSuccessfully: (status: boolean) => void;
  setUpdatedSuccessfully: (status: boolean) => void;
};

type Store = PiniaStore<'project', StoreState, StoreGetters, StoreActions>;

export const useProjectStore = (pinia: Pinia = piniaInstance): Store => {
  return useStore<Store, ApiStore<ProjectsApi>>(
    'project',
    {
      state: {
        projects: [],
        project: null,
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

        async findProjectById(
          id: ProjectModel['id'],
          { fullObjectNeeded = false },
        ) {
          let project: ProjectModel | DetailedProjectModel | null = null;
          if (!fullObjectNeeded)
            project =
              this.projects.find((project) => project.id === id) ?? null;
          else if (id === this.project?.id) project = this.project;
          if (!project) project = await this.fetch(id);
          return project;
        },

        async findProjectBySlug(
          slug: ProjectModel['slug'],
          { fullObjectNeeded = false },
        ) {
          let project: ProjectModel | DetailedProjectModel | null = null;
          if (!fullObjectNeeded)
            project =
              this.projects.find((project) => project.slug === slug) ?? null;
          else if (slug === this.project?.slug) project = this.project;
          if (!project) project = await this.fetchBySlug(slug);
          return project;
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

        async fetchBySlug(slug: ProjectModel['slug']) {
          try {
            this.setLoadingProject(true);
            const project: DetailedProjectModel = await this.callApi(
              'projectsSlugGet',
              {
                slug,
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
            const response = await this.callApi('projectsPut', {
              putProjectRequest: projectData,
            });
            if (response) {
              this.fetchAll();
              this.setAddedSuccessfully(true);
            } else {
              this.setAddedSuccessfully(false);
            }
          } finally {
            this.setLoadingAdd(false);
          }
        },

        async update(id: ProjectModel['id'], projectData: UpdateProjectModel) {
          try {
            this.setLoadingUpdate(true);
            this.setUpdatedSuccessfully(false);
            const response = await this.callApi('projectsPut', {
              putProjectRequest: projectData,
              projectId: id,
            });
            if (response) {
              this.setUpdatedSuccessfully(true);
              return this.fetch(id);
            } else {
              this.setUpdatedSuccessfully(false);
              return null;
            }
          } finally {
            this.setLoadingUpdate(false);
          }
        },

        async archive(id: ProjectModel['id']) {
          const project = await this.findProjectById(id, {
            fullObjectNeeded: true,
          });
          if (!project) throw new Error(`Project with id ${id} not found`);
          await this.update(id, { ...project, isArchived: true });
          await this.fetchAll();
        },

        async unarchive(id: ProjectModel['id']) {
          const project = await this.findProjectById(id, {
            fullObjectNeeded: true,
          });
          if (!project) throw new Error(`Project with id ${id} not found`);
          await this.update(id, { ...project, isArchived: false });
          await this.fetchAll();
        },

        async delete(id: ProjectModel['id']) {
          await this.callApi('projectsIdDelete', {
            id,
          });
          await this.fetchAll();
        },
      },
    },
    useApiStore(ProjectsApi, pinia),
  )(pinia);
};

type ProjectStore = ReturnType<typeof useProjectStore>;
export type { ProjectStore };
