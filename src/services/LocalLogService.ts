import type { LocalLogModel } from '@/models/Logs';
import { ApiService } from './ApiService';

class LocalLogService extends ApiService {
  fetchLocalLog = async (projectId: number): Promise<LocalLogModel[]> => {
    try {
      const response = await this.fetch('/Logs/' + projectId.toString(), {
        headers: {
          Accept: 'application/json',
          'Access-Control-Allow-Origin': '*',
          cors: 'cors',
        },
      });

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
