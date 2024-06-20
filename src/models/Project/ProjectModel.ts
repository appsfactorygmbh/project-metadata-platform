export type ProjectModel = {
  id: number;
  projectName: string;
  clientName: string;
  businessUnit: string;
  teamNumber: number;
};

export type DetailedProjectModel = ProjectModel & {
  department: string;
};
