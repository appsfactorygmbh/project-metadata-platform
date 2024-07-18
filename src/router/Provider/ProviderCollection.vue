<script setup lang="ts">
  import {
    ProjectProvider,
    PluginProvider,
    ProjectEditProvider,
    GlobalPluginProvider,
  } from '@/router/Provider';
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
