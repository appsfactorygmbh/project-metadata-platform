import { useProjectStore } from './ProjectsStore';
import { usePluginsStore } from './PluginStore';
import type { InjectionKey } from 'vue';
import type { useGlobalPluginsStore } from './GlobalPluginStore';

const projectsStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useProjectStore>
>;

const pluginStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof usePluginsStore>
>;

const globalPluginStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useGlobalPluginsStore>
>;

export { projectsStoreSymbol, pluginStoreSymbol, globalPluginStoreSymbol };
