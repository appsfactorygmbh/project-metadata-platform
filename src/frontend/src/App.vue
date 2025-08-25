<script setup lang="ts">
  import { ThemeProvider } from '@/router/Provider';
</script>

<template>
  <ThemeProvider>
    <RouterView />
  </ThemeProvider>
</template>
