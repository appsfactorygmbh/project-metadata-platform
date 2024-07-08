import type { PluginModel } from '@/models/Plugin';
class PluginService {
  fetchPlugins = async (projectID: number): Promise<PluginModel[]> => {
    console.log(projectID);
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL +
          '/Projects/' +
          projectID.toString() +
          '/plugins',
      );
      if (!response.ok) throw new Error('Error when trying to fetch Plugins');
      const data = await response.json();
      console.log('data from fetch: ', data);
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

  fetchGlobalPluginData = async (pluginId: number) => {
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
      keys: { key1: 'value1', key2: 'value2' },
    };
  };
}

const pluginService = new PluginService();
export { pluginService };
export type { PluginService };
