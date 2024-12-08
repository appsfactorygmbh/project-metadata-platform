import { useProjectStore } from './ProjectsStore';
import { usePluginsStore } from './PluginStore';
import { useProjectEditStore } from './ProjectEditStore/ProjectEditStore.ts';
import { useUserStore } from './UserStore';
import { useLogsStore } from './LogsStore';
import type { useGlobalPluginsStore } from './GlobalPluginStore';
import { useProjectRouting } from '@/utils/hooks/useProjectRouting.ts';
import { useLocalLogStore } from './LocalLogStore';
import type { InjectionKey } from 'vue';

const projectsStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useProjectStore>
>;

const pluginStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof usePluginsStore>
>;

const projectEditStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useProjectEditStore>
>;

const userStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useUserStore>
>;

const globalPluginStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useGlobalPluginsStore>
>;

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
  logsStoreSymbol,
  projectRoutingSymbol,
  localLogStoreSymbol,
};
