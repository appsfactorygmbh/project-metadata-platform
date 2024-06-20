import { useProjectStore } from './ProjectsStore';
import { usePluginsStore } from './PluginStore';
import type { InjectionKey } from 'vue';

const projectsStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof useProjectStore>
>;

const pluginStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof usePluginsStore>
>;

export { projectsStoreSymbol, pluginStoreSymbol };
