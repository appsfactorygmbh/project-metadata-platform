import type { PluginModel } from '@/models/Plugin';

export type GlobalPluginFormData = {
  pluginName: PluginModel['pluginName'];
  baseUrl: string;
};
