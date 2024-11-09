import type { PluginModel } from './PluginModel';

export type PluginEditModel = PluginModel & {
  editKey: number;
  isDeleted: boolean;
};
