import type { LogEntryModel } from '@/models/Log';
import { ApiService } from './ApiService';
import type { LogsApi } from '@/api/generated';

class LogsService extends ApiService<LogsApi> {
  fetchLocalLog = async (projectId: number): Promise<LogEntryModel[]> => {
    const response = await this.callApi('logsGet', {
      projectId,
    });
    if (!response) return [];
    return response;
  };
  fetchGlobalLogs = async (searchValue?: string): Promise<LogEntryModel[]> => {
    const response = await this.callApi('logsGet', {
      search: searchValue,
    });
    if (!response) return [];
    return response;
  };
}

const logsService = new LogsService('LogsService');
export { logsService };
export type { LogsService };
