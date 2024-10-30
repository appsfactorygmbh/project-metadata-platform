<script setup lang="ts">
  import {
    GlobalPluginProvider,
    PluginProvider,
    ProjectEditProvider,
    ProjectProvider,
    UserProvider,
  } from '@/router/Provider';
</script>

<template>
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
</template>
