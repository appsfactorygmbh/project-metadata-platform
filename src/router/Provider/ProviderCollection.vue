<script setup lang="ts">
  import {
    GlobalPluginProvider,
    PluginProvider,
    ProjectEditProvider,
    ProjectProvider,
    UserProvider,
  } from '@/router/Provider';
  import ProjectRoutingProvider from './ProjectRoutingProvider.vue';
  import LogsProvider from './LogsProvider.vue';
</script>

<template>
  <ProjectRoutingProvider>
    <LogsProvider>
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
  </LogsProvider>
  </ProjectRoutingProvider>
</template>
