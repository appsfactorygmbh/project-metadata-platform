import type { GlobalPluginModel } from '@/models/Plugin';
class GlobalPluginService {
  fetchGlobalPlugins = async (): Promise<GlobalPluginModel[]> => {
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL + '/Plugins',
      );
      if (!response.ok) throw new Error('Network response was not ok');
      const data = await response.json();
      return data;
    } catch (err) {
      console.error('Error fetching global plugins: ' + err);
      return [];
    }
  };

  removeGlobalPlugin = async (pluginId: number): Promise<Response | null> => {
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL + '/Plugins/' + pluginId.toString(),
        {
          method: 'DELETE',
          mode: 'cors',
        },
      );
      return response;
    } catch (err) {
      console.error('Error deleting global plugin: ' + err);
      return null;
    }
  };
}

const globalPluginService = new GlobalPluginService();
export { globalPluginService };
export type { GlobalPluginService };
