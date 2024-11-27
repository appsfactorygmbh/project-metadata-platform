<script setup lang="ts">
  import {
    AuthProvider,
    GlobalPluginProvider,
    ProjectEditProvider,
    UserProvider,
  } from '@/router/Provider';
  import ProjectRoutingProvider from './ProjectRoutingProvider.vue';
</script>

<template>
  <AuthProvider>
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
  </AuthProvider>
</template>
