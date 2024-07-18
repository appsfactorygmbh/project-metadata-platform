<script setup lang="ts">
  import {
    ProjectProvider,
    PluginProvider,
    GlobalPluginProvider,
    ProjectEditProvider,
  } from '@/router/Provider';
  import GlobalPluginProvider from './GlobalPluginProvider.vue';
</script>

<template>
  <ProjectEditProvider>
    <GlobalPluginProvider>
      <PluginProvider>
        <ProjectProvider>
          <slot>
            <RouterView></RouterView>
          </slot>
        </ProjectProvider>
      </PluginProvider>
    </GlobalPluginProvider>
  </ProjectEditProvider>
</template>
