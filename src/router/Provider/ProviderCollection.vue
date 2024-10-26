<script setup lang="ts">
  import {
    ProjectProvider,
    PluginProvider,
    ProjectEditProvider,
    GlobalPluginProvider,
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
