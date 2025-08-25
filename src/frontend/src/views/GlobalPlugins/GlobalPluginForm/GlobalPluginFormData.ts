import type { PluginModel } from '@/models/Plugin';
import type { GlobalPluginKey } from '@/models/GlobalPlugin';

export type GlobalPluginFormData = {
  pluginName: PluginModel['pluginName'];
  baseUrl: string;
  keys: GlobalPluginKey[];
};
