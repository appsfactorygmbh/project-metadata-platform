import type { GlobalPluginModel } from '@/models/Plugin';
class GlobalPluginService {
  patchGlobalPlugin = async (plugin: GlobalPluginModel) => {
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL + '/Plugins/' + plugin.id.toString(),
        {
          method: 'PATCH',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(plugin),
          mode: 'cors',
        },
      );
      return response;
    } catch (err) {
      console.error('Error patching global plugin: ' + err);
      return null;
    }
  };

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

  createGlobalPlugin = async (
    plugin: Omit<GlobalPluginModel, 'id'>,
  ): Promise<Response | null> => {
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL + '/Plugins',
        {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(plugin),
          mode: 'cors',
        },
      );
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
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL + '/Plugins/' + plugin.id.toString(),
        {
          method: 'PATCH',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(plugin),
          mode: 'cors',
        },
      );
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
