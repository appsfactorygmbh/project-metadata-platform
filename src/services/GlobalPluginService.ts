import type { GlobalPluginModel } from '@/models/Plugin';
import { ApiService } from './ApiService';
class GlobalPluginService extends ApiService {
  fetchGlobalPlugins = async (): Promise<GlobalPluginModel[]> => {
    try {
      const response = await this.fetch('/Plugins');
      if (!response.ok) throw new Error('Network response was not ok');
      const data = await response.json();
      return data;
    } catch (err) {
      console.error('Error fetching global plugins: ' + err);
      return [];
    }
  };

  archiveGlobalPlugin = async (pluginId: number): Promise<Response | null> => {
    try {
      const response = await this.fetch('/Plugins/' + pluginId.toString(), {
        method: 'PATCH',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ isArchived: true }),
        mode: 'cors',
      });
      return response;
    } catch (err) {
      console.error('Error deleting global plugin: ' + err);
      return null;
    }
  };

  createGlobalPlugin = async (
    plugin: Omit<GlobalPluginModel, 'id'>,
  ): Promise<Response | null> => {
    try {
      const response = await this.fetch('/Plugins', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(plugin),
        mode: 'cors',
      });
      return response;
    } catch (error) {
      console.error('Error:', error);
      return null;
    }
  };

  updateGlobalPlugin = async (
    plugin: GlobalPluginModel,
  ): Promise<Response | null> => {
    try {
      const response = await this.fetch('/Plugins/' + plugin.id.toString(), {
        method: 'PATCH',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(plugin),
        mode: 'cors',
      });
      return response;
    } catch (error) {
      console.error('Error:', error);
      return null;
    }
  };
}

const globalPluginService = new GlobalPluginService();
export { globalPluginService };
export type { GlobalPluginService };
