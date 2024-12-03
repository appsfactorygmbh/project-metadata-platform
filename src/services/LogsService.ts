import type { LogEntryModel } from '@/models/Log';
import { ApiService } from './ApiService';

class LogsService extends ApiService {
  fetchLocalLog = async (projectId: number): Promise<LogEntryModel[]> => {
    try {
      const response = await this.fetch(
        '/logs?projectId=' + projectId.toString(),
      );
      if (!response.ok)
        throw new Error('Error while trying to fetch local logs');

      const data: LogEntryModel[] = await response.json();
      return data;
    } catch (err) {
      console.error('Error fetching local log: ' + err);
      return [];
    }
  };
  fetchGlobalLogs = async (searchValue?: string): Promise<LogEntryModel[]> => {
    const url = searchValue ? `/logs?search=${searchValue}` : '/logs';
    try {
      const response = await this.fetch(url);
      if (!response.ok)
        throw new Error('Error while trying to fetch global logs');
      const data = await response.json();
      return data;
    } catch (error) {
      console.log(error);
      return [];
    }
  };
}

const logsService = new LogsService();
export { logsService };
export type { LogsService };
