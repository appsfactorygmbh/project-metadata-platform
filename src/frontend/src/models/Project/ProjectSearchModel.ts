import type { SecurityLevel } from '@/api/generated';

export interface ProjectSearchModel {
  id: number;
  slug: string;
  projectName: string;
  clientName: string;
  companyName: string;
  isArchived: boolean;
  ismsLevel: SecurityLevel;
  teamName: string;
  businessUnit: string;
}
