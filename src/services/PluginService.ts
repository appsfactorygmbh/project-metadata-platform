import type { GlobalPluginModel, PluginModel } from '@/models/Plugin';
class PluginService {
  fetchPlugins = async (projectID: number): Promise<PluginModel[]> => {
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL +
          '/Projects/' +
          projectID.toString() +
          '/plugins',
      );
      if (!response.ok) throw new Error('Error when trying to fetch Plugins');
      const data = await response.json();
      return data;
    } catch (error) {
      console.log(error);
      return [];
    }
  };

  createPlugin = async (plugin: PluginModel): Promise<void> => {
    //TODO: implement function
    console.log(plugin);
  };

  fetchGlobalPlugin = async (pluginId: GlobalPluginModel['id']) => {
    //   try {
    //     const response = await fetch(
    //       import.meta.env.VITE_BACKEND_URL + '/plugins/' + pluginId.toString(),
    //     );
    //     if (!response.ok)
    //       throw new Error('Error when trying to fetch global Plugin');
    //     const data = await response.json();
    //     console.log('data from fetch: ', data);
    //     return data;
    //   } catch (error) {
    //     console.log(error);
    //     return [];
    //   }
    // };

    /* mock of function, actual function body is above */
    return {
      id: pluginId,
      pluginName: 'Test Plugin',
      keys: [
        { key: 'value1', archived: false },
        { key: 'value2', archived: false },
      ],
    };
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

  removeGlobalPlugin = async (
    pluginId: GlobalPluginModel['id'],
  ): Promise<Response | null> => {
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL + '/Plugins/' + pluginId.toString(),
        {
          method: 'DELETE',
        },
      );
      return response;
    } catch (err) {
      console.error('Error deleting global plugin: ' + err);
      return null;
    }
  };
}

const pluginService = new PluginService();
export { pluginService };
export type { PluginService };
