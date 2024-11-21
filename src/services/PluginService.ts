import type { PluginModel } from '@/models/Plugin';
import { ApiService } from './ApiService';
class PluginService extends ApiService {
  fetchPlugins = async (projectID: number): Promise<PluginModel[]> => {
    try {
      const response = await this.fetch(
        '/Projects/' + projectID.toString() + '/plugins',
      );
      if (!response.ok) throw new Error('Error when trying to fetch Plugins');
      const data = await response.json();
      return data;
    } catch (error) {
      console.error('Error when trying to fetch Plugins' + error);
      return [];
    }
  };

  fetchUnarchivedPlugins = async (
    projectId: number,
  ): Promise<PluginModel[]> => {
    try {
      const response = await this.fetch(
        '/Projects/' + projectId.toString() + '/UnarchivedPlugins',
      );
      if (!response.ok)
        throw new Error('Error when trying to fetch Unarchived Plugins');
      const data = await response.json();
      return data;
    } catch (error) {
      console.error('Error when trying to fetch Unarchived Plugins' + error);
      return [];
    }
  };
}

const pluginService = new PluginService();
export { pluginService };
export type { PluginService };
