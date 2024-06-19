<script setup lang="ts">
  import {
    ThemeProvider,
    ProjectProvider,
    ProjectInformationProvider,
  } from '@/router/Provider';
</script>

<template>
  <ThemeProvider>
    <ProjectProvider>
      <ProjectInformationProvider>
        <RouterView />
      </ProjectInformationProvider>
    </ProjectProvider>
  </ThemeProvider>
</template>

<style scoped></style>
