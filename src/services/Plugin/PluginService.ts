interface Plugin {
  pluginName: string;
  url: string;
  displayName: string;
  id: string;
}
class PluginService {
  fetchPlugins = async (projectID: string): Promise<Plugin[]> => {
    console.log(projectID);
    try {
      const response = await fetch('http://localhost:3000/plugins/');
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
