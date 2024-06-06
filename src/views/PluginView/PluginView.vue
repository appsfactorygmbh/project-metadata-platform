<template>
  <div class="container">
    <!-- Iterate over each plugin in the array and display using PluginComponent -->
    <PluginComponent
      v-for="plugin in plugins"
      :key="plugin.name"
      class="plugins"
      :plugin-name="plugin.name"
      :url="plugin.url"
    ></PluginComponent>
  </div>
</template>

<script setup lang="ts">
  import { ref } from 'vue';
  import PluginComponent from '../../components/Plugin/PluginComponent.vue';

  // Define an array of plugins with their names and URLs for demonstration.
  const plugins = ref([
    {
      name: 'GitLab',
      url: 'gitlab.com/path/to/project',
    },
    {
      name: 'GitHub',
      url: 'github.com/path/to/project',
    },
    {
      name: 'Azure DevOps',
      url: 'https://azure.microsoft.com/de-de/products/devops',
    },
    {
      name: 'Google Drive',
      url: 'https://drive.google.com/drive/path/to/own/drive',
    },
    {
      name: 'Salesforce',
      url: 'https://www.salesforce.com/de/',
    },
  ]);
</script>

<style scoped lang="css">
  /* Styling for the container */
  .container {
    width: 100vw;
    height: auto;
    display: flex;
    justify-content: left;
    align-items: center;
    flex-direction: row;
    flex-wrap: wrap;
  }
  /* Styling for each plugin in container */
  .plugins {
    margin: 10px;
  }
</style>
