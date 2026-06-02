import {
  AuthApi,
  type AuthApi as AuthApiType,
  BusinessUnitsApi,
  type BusinessUnitsApi as BusinessUnitsApiType,
  CompaniesApi,
  type CompaniesApi as CompaniesApiType,
  DepartmentsApi,
  type DepartmentsApi as DepartmentsApiType,
  LogsApi,
  type LogsApi as LogsApiType,
  OfficeLocationsApi,
  type OfficeLocationsApi as OfficeLocationsApiType,
  PluginsApi,
  type PluginsApi as PluginsApiType,
  ProjectsApi,
  type ProjectsApi as ProjectsApiType,
  TeamsApi,
  type TeamsApi as TeamsApiType,
  UsersApi,
  type UsersApi as UsersApiType,
} from './generated';

export type ApiTypes =
  | AuthApiType
  | ProjectsApiType
  | PluginsApiType
  | UsersApiType
  | LogsApiType
  | TeamsApiType
  | CompaniesApiType
  | DepartmentsApiType
  | OfficeLocationsApiType
  | BusinessUnitsApiType;

export type ApiInstance<T extends ApiTypes> = T extends AuthApiType
  ? typeof AuthApi
  : T extends ProjectsApiType
    ? typeof ProjectsApi
    : T extends PluginsApiType
      ? typeof PluginsApi
      : T extends UsersApiType
        ? typeof UsersApi
        : T extends LogsApiType
          ? typeof LogsApi
          : T extends TeamsApiType
            ? typeof TeamsApi
            : T extends BusinessUnitsApiType
              ? typeof BusinessUnitsApi
              : T extends CompaniesApiType
                ? typeof CompaniesApi
                : T extends DepartmentsApiType
                  ? typeof DepartmentsApi
                  : T extends OfficeLocationsApiType
                    ? typeof OfficeLocationsApi
                    : never;
