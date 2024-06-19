<script setup lang="ts">
  import {
    ThemeProvider,
    ProjectProvider,
    ProjectInformationProvider,
    PluginProvider
  } from '@/router/Provider';
</script>

<template>
  <ThemeProvider>
  <PluginProvider>
    <ProjectProvider>
      <ProjectInformationProvider>
        <RouterView />
      </ProjectInformationProvider>
    </ProjectProvider>
  <PluginProvider />
  </ThemeProvider>
</template>

<style scoped></style>
