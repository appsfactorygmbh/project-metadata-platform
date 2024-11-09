<script setup lang="ts">
  import {
    AuthProvider,
    GlobalPluginProvider,
    PluginProvider,
    ProjectEditProvider,
    UserProvider,
  } from '@/router/Provider';
</script>

<template>
  <AuthProvider>
    <UserProvider>
      <PluginProvider>
        <ProjectEditProvider>
          <GlobalPluginProvider>
            <slot>
              <RouterView />
            </slot>
          </GlobalPluginProvider>
        </ProjectEditProvider>
      </PluginProvider>
    </UserProvider>
  </AuthProvider>
</template>
