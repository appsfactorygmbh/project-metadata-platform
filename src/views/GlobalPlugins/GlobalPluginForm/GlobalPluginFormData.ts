import type { PluginModel, ProjectKey } from '@/models/Plugin';

export type GlobalPluginFormData = {
  pluginName: PluginModel['pluginName'];
  keys: ProjectKey[];
};
