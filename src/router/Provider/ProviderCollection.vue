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
  import LogsProvider from './LogsProvider.vue';
</script>

<template>
  <ProjectRoutingProvider>
    <UserProvider>
      <LogsProvider>
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
      </LogsProvider>
    </UserProvider>
  </ProjectRoutingProvider>
</template>
