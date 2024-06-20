import type { ProjectModel } from './ProjectModel';

//Data type for the ProjectInformation
export type ProjectDetailedModel = ProjectModel & {
  department: string;
};
