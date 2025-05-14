import type { PatchTeamRequest } from '@/api/generated';

export type PluginEditModel = PatchTeamRequest & {
  editKey: number;
  isDeleted: boolean;
};
