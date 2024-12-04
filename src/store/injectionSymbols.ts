import { type ProjectsStore } from './ProjectsStore';
import { type PluginStore } from './PluginStore';
import { type ProjectEditStore } from './ProjectEditStore/ProjectEditStore.ts';
import { type UserStore } from './UserStore';
import { useLogsStore } from './LogsStore';
import { useProjectRouting } from '@/utils/hooks/useProjectRouting.ts';
import { useLocalLogStore } from './LocalLogStore';
import type { InjectionKey } from 'vue';
// import type { useAuthStore } from './AuthStore.ts';
import type { GlobalPluginsStore } from './GlobalPluginStore.ts';
import type { AuthStore } from './AuthStore.ts';

const projectsStoreSymbol = Symbol() as InjectionKey<ProjectsStore>;

const pluginStoreSymbol = Symbol() as InjectionKey<PluginStore>;

const projectEditStoreSymbol = Symbol() as InjectionKey<ProjectEditStore>;

const userStoreSymbol = Symbol() as InjectionKey<UserStore>;

const globalPluginStoreSymbol = Symbol() as InjectionKey<GlobalPluginsStore>;

const authStoreSymbol = Symbol() as InjectionKey<AuthStore>;

const logsStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useLogsStore>
>;

const projectRoutingSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useProjectRouting>
>;

const localLogStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useLocalLogStore>
>;

export {
  projectsStoreSymbol,
  pluginStoreSymbol,
  globalPluginStoreSymbol,
  projectEditStoreSymbol,
  userStoreSymbol,
  authStoreSymbol,
  logsStoreSymbol,
  projectRoutingSymbol,
  localLogStoreSymbol,
};
