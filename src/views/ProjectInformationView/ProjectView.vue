<script lang="ts" setup>
  import { PluginView } from '@/views/PluginView';
  import { ProjectInformation } from './ProjectInformation';
  import {
    pluginStoreSymbol,
    projectsStoreSymbol,
  } from '@/store/injectionSymbols';
  import { inject, onMounted } from 'vue';

  const projectsStore = inject(projectsStoreSymbol);
  const pluginStore = inject(pluginStoreSymbol);

  onMounted(async () => {
    const projectId = projectsStore?.getProjects[0].id || 100;
    await projectsStore?.fetchProject(projectId);
    await pluginStore?.fetchPlugins(projectId);
  });
</script>

<template>
  <ProjectInformation />
  <PluginView class="pluginView" />
</template>

<style scoped lang="scss">
  /* Style for the middle section */
  .main {
    width: 100%;
    max-height: 80vh;
    height: max-content;
    padding-right: 5em;
    padding-left: 5em;

    display: flex;
    flex-direction: column;
    align-items: center;
  }

  .pluginView {
    padding: 0;
  }

  /* Style for the right panel */
  .pane {
    display: flex;
    flex-direction: row;
  }

  /* Style for the Project name input box */
  .projectNameInput {
    font-size: 2.8em;
    width: 80%;
    height: 2.8em;
    text-align: center;
    border: none;
    border-bottom: 2px solid #a5a4a4;
    color: black;
    background-color: rgb(250, 250, 250);
  }

  /* Style for the Project title box */
  .projectNameContainer {
    width: 100%;
    height: 5%;
    margin: 10px;
    border-radius: 10px;
    text-align: center;
    align-items: center;
    flex-direction: row;
    display: flex;
    justify-content: center;
  }

  .projectName {
    font-size: 2.5em;
    font-weight: bold;
    color: #000;
    margin: 10px;
  }

  .projectInformationBox {
    width: 100%;
    height: auto;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: row;
    flex-wrap: wrap;
    padding-top: 1em;
    padding-bottom: 1em;
    border-radius: 10px;

    background: white;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  }

  .infoCard {
    border: none;
    width: 50%;
    display: table;
    padding-left: 1em;
    padding-right: 1em;
  }

  .button {
    margin-bottom: 10px;
    height: 50px;
    width: 50px;
    border: none;
  }
  .button {
    height: 40px;
    width: 40px;
    border: none;
  }

  .icon {
    color: black; //TODO: change to appsfactory grey
    font-size: 2.5em;
  }

  .label {
    font-size: 1.4em;
    font-weight: bold;
    margin: 0 0 0 auto;
  }

  .projectInfo {
    font-size: 1.4em;
    margin: 0 auto 0 1em;
    white-space: nowrap;
  }

  .pluginView {
    display: flex;
    justify-content: center;
    padding-top: 1em;
  }
</style>
