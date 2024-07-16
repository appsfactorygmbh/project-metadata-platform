<script setup lang="ts">
  import {
    ProjectProvider,
    PluginProvider,
    GlobalPluginProvider,
  } from '@/router/Provider';
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
