<script lang="ts" setup>
  import { ProjectPlugins } from './ProjectPlugins';
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
  <ProjectPlugins class="pluginView" />
</template>

<style scoped>
  .pluginView {
    display: flex;
    justify-content: center;
    padding-top: 1em;
  }
</style>
