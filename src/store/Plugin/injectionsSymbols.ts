import { usePluginsStore } from './PluginStore';
import type { InjectionKey } from 'vue';

const pluginStoreSymbol = Symbol() as InjectionKey<
  ReturnType<typeof usePluginsStore>
>;

export { pluginStoreSymbol };
