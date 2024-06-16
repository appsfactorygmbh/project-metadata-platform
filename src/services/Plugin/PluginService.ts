import { Plugin } from '../../models/Plugin.ts';
class PluginService {
  fetchPlugins = async (projectID: string): Promise<Plugin[]> => {
    console.log(projectID);
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL + '/Plugins?id=' + projectID,
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
}

const pluginService = new PluginService();
export { pluginService };
