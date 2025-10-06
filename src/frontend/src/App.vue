<script setup lang="ts">
  import { ThemeProvider } from '@/router/Provider';
</script>

<template>
  <ThemeProvider>
    <Suspense>
      <RouterView />
    </Suspense>
  </ThemeProvider>
</template>
