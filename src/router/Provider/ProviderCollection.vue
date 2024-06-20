<script setup lang="ts">
  import { ProjectProvider, PluginProvider } from '@/router/Provider';
</script>

<template>
  <PluginProvider>
    <ProjectProvider>
      <slot>
        <RouterView></RouterView>
      </slot>
    </ProjectProvider>
  </PluginProvider>
</template>
