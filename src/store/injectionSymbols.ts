import { useProjectStore } from './ProjectsStore';
import { usePluginsStore } from './PluginStore';
import { useProjectEditStore } from './ProjectEditStore';
import type { useGlobalPluginsStore } from './GlobalPluginStore';
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

const globalPluginStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useGlobalPluginsStore>
>;

export {
  projectsStoreSymbol,
  pluginStoreSymbol,
  globalPluginStoreSymbol,
  projectEditStoreSymbol,
};
