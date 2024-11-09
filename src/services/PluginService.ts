import { ApiService } from './ApiService';
import type { ProjectsApi } from '@/api/generated';
class PluginService extends ApiService<ProjectsApi> {
  fetchPlugins = async (projectID: number) => {
    const plugins = await this.callApi('projectsIdPluginsGet', {
      id: projectID,
    });
    if (!plugins) return [];
    return plugins;
  };
}

const pluginService = new PluginService('PluginService');
export { pluginService };
export type { PluginService };
