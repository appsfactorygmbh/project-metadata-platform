import type { PluginModel } from '@/models/Plugin';

export type UpdateProjectModel = {
  projectName: string | undefined;
  businessUnit: string | undefined;
  teamNumber: number | undefined;
  department: string | undefined;
  clientName: string | undefined;
  pluginList: PluginModel[] | undefined;
};
