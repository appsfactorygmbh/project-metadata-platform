import type { PluginModel, GlobalPluginKey } from '@/models/Plugin';

export type GlobalPluginFormData = {
  pluginName: PluginModel['pluginName'];
  keys: GlobalPluginKey[];
};
