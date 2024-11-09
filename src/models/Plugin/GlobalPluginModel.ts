import type { GetGlobalPluginResponse } from '@/api/generated';

export type GlobalPluginModel = GetGlobalPluginResponse;
// {
//   name: string;
//   id: number;
//   archived: boolean;
//   keys: string[];
// };

export type GlobalPluginKey = {
  value: string;
  key: number;
  archived: boolean;
};
