import { type PluginsStore, usePluginsStore } from './PluginStore';
import {
  type ProjectEditStore,
  useProjectEditStore,
} from './ProjectEditStore/ProjectEditStore.ts';
import { type ProjectsStore, useProjectStore } from './ProjectsStore';
import { type SearchStore, useSearchStore } from './SearchStore';
// import {
//   type GlobalPluginsStore,
//   useGlobalPluginsStore,
// } from './GlobalPluginStore';
import { type UserStore, useUserStore } from './UserStore';

export {
  usePluginsStore,
  useProjectStore,
  useSearchStore,
  // useGlobalPluginsStore,
  useProjectEditStore,
  useUserStore,
};
export type {
  PluginsStore,
  ProjectsStore,
  SearchStore,
  // GlobalPluginsStore,
  ProjectEditStore,
  UserStore,
};
