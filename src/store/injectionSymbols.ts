import { useProjectStore } from './ProjectsStore';
import { usePluginsStore } from './PluginStore';
import { useProjectEditStore} from "@/store/ProjectEditStore.ts";
import type { InjectionKey } from 'vue';
import type { useGlobalPluginsStore } from './GlobalPluginStore';

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

export { projectsStoreSymbol, pluginStoreSymbol, globalPluginStoreSymbol, projectEditStoreSymbol };
