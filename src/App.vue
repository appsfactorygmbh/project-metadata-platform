<script setup lang="ts">
  import { ThemeProvider, ProjectProvider } from '@/router/Provider';
</script>

<template>
  <ThemeProvider>
    <ProjectProvider>
      <RouterView />
    </ProjectProvider>
  </ThemeProvider>
</template>

<style scoped></style>
