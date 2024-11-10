<script setup lang="ts">
  import {
    GlobalPluginProvider,
    PluginProvider,
    ProjectEditProvider,
    ProjectProvider,
    UserProvider,
  } from '@/router/Provider';
  import LogsProvider from './LogsProvider.vue';
</script>

<template>
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
</template>
