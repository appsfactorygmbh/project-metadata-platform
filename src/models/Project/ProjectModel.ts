import type { GetProjectResponse, GetProjectsResponse } from '@/api/generated';

// TODO: Remove this once implemented in backend
export type ProjectModel = GetProjectsResponse & {
  isArchived: boolean;
};

export type DetailedProjectModel = GetProjectResponse & {
  isArchived: boolean;
};
