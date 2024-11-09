import {
  AuthApi,
  type AuthApi as AuthApiType,
  PluginsApi,
  type PluginsApi as PluginsApiType,
  ProjectsApi,
  type ProjectsApi as ProjectsApiType,
  UsersApi,
  type UsersApi as UsersApiType,
} from './generated';

export type ApiTypes =
  | AuthApiType
  | ProjectsApiType
  | PluginsApiType
  | UsersApiType;

export type ApiInstance<T extends ApiTypes> = T extends AuthApiType
  ? typeof AuthApi
  : T extends ProjectsApiType
    ? typeof ProjectsApi
    : T extends PluginsApiType
      ? typeof PluginsApi
      : T extends UsersApiType
        ? typeof UsersApi
        : never;
