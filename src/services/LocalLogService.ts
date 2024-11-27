import type { LocalLogModel } from '@/models/LocalLogs';
import { ApiService } from './ApiService';

class LocalLogService extends ApiService {
  fetchLocalLog = async (projectId: number): Promise<LocalLogModel[]> => {
    try {
      const response = await this.fetch('/logs?projectId=' + projectId.toString(), {
        headers: {
          Accept: 'application/json',
          'Access-Control-Allow-Origin': '*',
          cors: 'cors',
        },
      });
      if (!response.ok)
        throw new Error('Error while trying to fetch local logs');

      const data: LocalLogModel[] = await response.json();
      return data;
    } catch (err) {
      console.error('Error fetching local log: ' + err);
      return [];
    }
  };
}

const localLogService = new LocalLogService();
export { localLogService };
export type { LocalLogService };
