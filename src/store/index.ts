import { type PluginStore, usePluginStore } from './PluginStore';
import {
  type ProjectEditStore,
  useProjectEditStore,
} from './ProjectEditStore/ProjectEditStore.ts';
import { type ProjectsStore, useProjectStore } from './ProjectsStore';
import { type SearchStore, useSearchStore } from './SearchStore';
import {
  type GlobalPluginsStore,
  useGlobalPluginsStore,
} from './GlobalPluginStore';
import { type UserStore, useUserStore } from './UserStore';
import { type AuthStore, useAuthStore } from './AuthStore';
import { type LocalLogStore, useLocalLogStore } from './LocalLogStore';
import { type LogsStore, useLogsStore } from './LogsStore';

export {
  useAuthStore,
  useProjectStore,
  usePluginStore,
  useSearchStore,
  useGlobalPluginsStore,
  useProjectEditStore,
  useUserStore,
  useLocalLogStore,
  useLogsStore,
};
export type {
  PluginStore,
  ProjectsStore,
  SearchStore,
  GlobalPluginsStore,
  ProjectEditStore,
  UserStore,
  AuthStore,
  LocalLogStore,
  LogsStore,
};
