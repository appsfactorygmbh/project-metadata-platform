import type {
  GetProjectDetailsResponse,
  GetProjectResponse,
  GetProjectResponseGetListResponse,
} from '@/api/generated';

export type ProjectModel = GetProjectResponse;

export type ProjectListModel = GetProjectResponseGetListResponse;

export type DetailedProjectModel = GetProjectDetailsResponse;
