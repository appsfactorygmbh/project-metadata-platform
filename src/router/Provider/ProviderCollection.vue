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
  import UserRoutingProvider from './UserRoutingProvider.vue';
</script>

<template>
  <UserRoutingProvider>
    <AuthProvider>
      <ProjectRoutingProvider>
        <UserProvider>
          <LogsProvider>
            <LocalLogProvider>
              <ProjectEditProvider>
                <GlobalPluginProvider>
                  <slot>
                    <RouterView />
                  </slot>
                </GlobalPluginProvider>
              </ProjectEditProvider>
            </LocalLogProvider>
          </LogsProvider>
        </UserProvider>
      </ProjectRoutingProvider>
    </AuthProvider>
  </UserRoutingProvider>
</template>
