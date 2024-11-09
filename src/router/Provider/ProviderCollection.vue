<script setup lang="ts">
  import {
    AuthProvider,
    GlobalPluginProvider,
    PluginProvider,
    ProjectEditProvider,
    ProjectProvider,
    UserProvider,
  } from '@/router/Provider';
</script>

<template>
  <AuthProvider>
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
  </AuthProvider>
</template>
