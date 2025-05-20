import {
  AuthApi,
  type AuthApi as AuthApiType,
  LogsApi,
  type LogsApi as LogsApiType,
  PluginsApi,
  type PluginsApi as PluginsApiType,
  ProjectsApi,
  type ProjectsApi as ProjectsApiType,
  UsersApi,
  type UsersApi as UsersApiType,
  TeamsApi,
  type TeamsApi as TeamsApiType,
} from './generated';

export type ApiTypes =
  | AuthApiType
  | ProjectsApiType
  | PluginsApiType
  | UsersApiType
  | LogsApiType
  | TeamsApiType;

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
            : never;
