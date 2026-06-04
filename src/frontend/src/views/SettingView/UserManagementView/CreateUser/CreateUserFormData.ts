export type CreateUserFormData = {
  employeeNumber: string;
  email: string;
  password: string;
  confirmPassword: string;
  active: boolean;
  jobTitles: string[];
  teams: string[];
  teamSupport: string[];
  departments: string[];
  businessUnits: string[];
  company: string | undefined;
  officeLocation: string | undefined;
  inputsDisabled: boolean;
};
