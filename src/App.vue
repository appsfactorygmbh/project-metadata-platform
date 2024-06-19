<script setup lang="ts">
  import {
    ThemeProvider,
    ProjectProvider,
    PluginProvider,
  } from '@/router/Provider';
</script>

<template>
  <ThemeProvider>
    <PluginProvider>
      <ProjectProvider>
        <RouterView />
      </ProjectProvider>
    </PluginProvider>
  </ThemeProvider>
</template>

<style scoped></style>
