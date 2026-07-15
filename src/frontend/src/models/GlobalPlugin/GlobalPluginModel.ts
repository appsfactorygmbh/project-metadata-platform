import type {
  GetGlobalPluginResponse,
  GetGlobalPluginResponseGetListResponse,
} from '@/api/generated';

export type GlobalPluginModel = GetGlobalPluginResponse;

export type GlobalPluginListModel = GetGlobalPluginResponseGetListResponse;

export type GlobalPluginKey = {
  value: string;
  key: number;
  archived: boolean;
};
