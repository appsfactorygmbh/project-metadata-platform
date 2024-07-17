import type { PluginModel } from '@/models/Plugin';

export type UpdateProjectModel = {
  projectName: string;
  businessUnit: string;
  teamNumber: number;
  department: string;
  clientName: string;
  pluginList: PluginModel[] | [];
};
