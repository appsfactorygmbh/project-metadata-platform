export type ProjectModel = {
  id: number;
  projectName: string;
  clientName: string;
  businessUnit: string;
  teamNumber: number;
  isArchived: boolean;
};

export type DetailedProjectModel = ProjectModel & {
  department: string;
};
