<script setup lang="ts">
  import {
    AuthProvider,
    GlobalPluginProvider,
    LocalLogProvider,
    ProjectEditProvider,
    UserProvider,
    TeamProvider,
  } from '@/router/Provider';
  import ProjectRoutingProvider from './ProjectRoutingProvider.vue';
  import LogsProvider from './LogsProvider.vue';
  import UserRoutingProvider from './UserRoutingProvider.vue';
  import TeamRoutingProvider from './TeamRoutingProvider.vue';
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
                  <TeamRoutingProvider>
                    <TeamProvider>
                      <slot>
                        <RouterView />
                      </slot>
                    </TeamProvider>
                  </TeamRoutingProvider>
                </GlobalPluginProvider>
              </ProjectEditProvider>
            </LocalLogProvider>
          </LogsProvider>
        </UserProvider>
      </ProjectRoutingProvider>
    </AuthProvider>
  </UserRoutingProvider>
</template>
