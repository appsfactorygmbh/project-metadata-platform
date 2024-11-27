import type { GetGlobalPluginResponse } from '@/api/generated';

export type GlobalPluginModel = GetGlobalPluginResponse;

export type GlobalPluginKey = {
  value: string;
  key: number;
  archived: boolean;
};
