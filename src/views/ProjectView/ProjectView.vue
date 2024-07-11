<script lang="ts" setup>
  import { ProjectPlugins } from './ProjectPlugins';
  import { ProjectInformation } from './ProjectInformation';
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
