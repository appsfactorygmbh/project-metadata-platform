import type { UpdateProjectModel } from './UpdateProjectModel';

export type EditProjectModel = Omit<
  UpdateProjectModel,
  'isArchived' | 'pluginList'
>;
