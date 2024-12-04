<script setup lang="ts">
  import {
    AuthProvider,
    GlobalPluginProvider,
    LocalLogProvider,
    ProjectEditProvider,
    UserProvider,
  } from '@/router/Provider';
  import ProjectRoutingProvider from './ProjectRoutingProvider.vue';
  import LogsProvider from './LogsProvider.vue';
</script>

<template>
  <AuthProvider>
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
  </AuthProvider>
</template>
