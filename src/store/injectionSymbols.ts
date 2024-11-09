import { type ProjectsStore } from './ProjectsStore';
import { type PluginsStore } from './PluginStore';
import { type ProjectEditStore } from './ProjectEditStore/ProjectEditStore.ts';
import { type UserStore } from './UserStore';
// import type { useGlobalPluginsStore } from './GlobalPluginStore';
import type { InjectionKey } from 'vue';
// import type { useAuthStore } from './AuthStore.ts';
import type { apiStore } from './ApiStore.ts';
import type { globalPluginsStore } from './GlobalPluginStore.ts';
import type { AuthStore } from './AuthStore.ts';

const projectsStoreSymbol = Symbol() as InjectionKey<ProjectsStore>;

const pluginStoreSymbol = Symbol() as InjectionKey<PluginsStore>;

const projectEditStoreSymbol = Symbol() as InjectionKey<ProjectEditStore>;

const userStoreSymbol = Symbol() as InjectionKey<UserStore>;

const globalPluginStoreSymbol = Symbol() as InjectionKey<
  typeof globalPluginsStore
>;

const authStoreSymbol = Symbol() as InjectionKey<AuthStore>;

const apiStoreSymbol = Symbol() as InjectionKey<ReturnType<typeof apiStore>>;

export {
  projectsStoreSymbol,
  pluginStoreSymbol,
  globalPluginStoreSymbol,
  projectEditStoreSymbol,
  userStoreSymbol,
  authStoreSymbol,
  apiStoreSymbol,
};
