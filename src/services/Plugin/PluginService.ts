import type { PluginType } from '@/models/PluginType.ts';
class PluginService {
  fetchPlugins = async (projectID: number): Promise<PluginType[]> => {
    try {
      const response = await fetch(
        import.meta.env.VITE_BACKEND_URL +
          '/Plugins?id=' +
          projectID.toString(),
      );
      if (!response.ok) throw new Error('Error when trying to fetch Plugins');
      const data = await response.json();
      return data;
    } catch (error) {
      console.log(error);
      return [];
    }
  };
}

const pluginService = new PluginService();
export { pluginService };
