<script setup lang="ts">
  import {
    GlobalPluginProvider,
    PluginProvider,
    ProjectEditProvider,
    ProjectProvider,
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
