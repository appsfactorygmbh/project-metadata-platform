<script setup lang="ts">
  import { PluginProvider, ThemeProvider } from '@/router/Provider';
</script>

<template>
  <ThemeProvider>
    <PluginProvider>
      <RouterView />
    </PluginProvider>
  </ThemeProvider>
</template>

<style scoped></style>
