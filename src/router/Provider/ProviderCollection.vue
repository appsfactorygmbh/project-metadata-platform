<script setup lang="ts">
  import {
    GlobalPluginProvider,
    PluginProvider,
    ProjectEditProvider,
    ProjectProvider,
    UserProvider,
  } from '@/router/Provider';
  import ProjectRoutingProvider from './ProjectRoutingProvider.vue';
</script>

<template>
  <ProjectRoutingProvider>
    <UserProvider>
      <ProjectProvider>
        <PluginProvider>
          <ProjectEditProvider>
            <GlobalPluginProvider>
              <slot>
                <RouterView />
              </slot>
            </GlobalPluginProvider>
          </ProjectEditProvider>
        </PluginProvider>
      </ProjectProvider>
    </UserProvider>
  </ProjectRoutingProvider>
</template>
