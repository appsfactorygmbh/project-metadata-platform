import type { SecurityLevel } from "@/api/generated";

export interface ProjectSearchModel{
    id: number;
    slug: string;
    projectName: string;
    clientName: string;
    company: string;
    isArchived: boolean;
    ismsLevel: SecurityLevel;
    teamName: string;
    businessUnit: string;
  }
