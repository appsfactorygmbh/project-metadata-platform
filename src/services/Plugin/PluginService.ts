import type { PluginType } from '../../models/PluginType.ts';
class PluginService {
  fetchPlugins = async (projectID: number): Promise<PluginType[]> => {
    console.log(projectID);
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL + '/Plugins?id=' + projectID.toString(),
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
