<script setup lang="ts">
  import {
    AuthProvider,
    GlobalPluginProvider,
    ProjectEditProvider,
    UserProvider,
  } from '@/router/Provider';
</script>

<template>
  <AuthProvider>
    <UserProvider>
      <ProjectEditProvider>
        <GlobalPluginProvider>
          <slot>
            <RouterView />
          </slot>
        </GlobalPluginProvider>
      </ProjectEditProvider>
    </UserProvider>
  </AuthProvider>
</template>
