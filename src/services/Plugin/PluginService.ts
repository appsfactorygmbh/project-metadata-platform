class PluginService {
  fetchPlugins = (projectID: string) => {
    fetch(import.meta.env.VITE_BACKEND_URL + '/Plugins/' + projectID)
      .then(response => {
        if(!response.ok) throw new Error ("Error when trying to fetch Plugins")
        return response.json()
      })
      .then(data => {
        console.log(data)
      })
      .catch(error => {
        console.log(error)
      })
  }
}

const pluginService = new PluginService()
export {pluginService}