<script setup lang="ts">
  import {
    GlobalPluginProvider,
    LocalLogProvider,
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
      <LocalLogProvider>
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
      </LocalLogProvider>
    </UserProvider>
  </ProjectRoutingProvider>
</template>
