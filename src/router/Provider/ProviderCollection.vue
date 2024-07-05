<script setup lang="ts">
  import { ProjectProvider, PluginProvider } from '@/router/Provider';
  import GlobalPluginProvider from './GlobalPluginProvider.vue';
</script>

<template>
  <GlobalPluginProvider>
    <PluginProvider>
      <ProjectProvider>
        <slot>
          <RouterView></RouterView>
        </slot>
      </ProjectProvider>
    </PluginProvider>
  </GlobalPluginProvider>
</template>
