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
      <TeamProvider>
        <ProjectRoutingProvider>
          <UserProvider>
            <LogsProvider>
              <LocalLogProvider>
                <ProjectEditProvider>
                  <GlobalPluginProvider>
                    <TeamRoutingProvider>
                      <slot>
                        <RouterView />
                      </slot>
                    </TeamRoutingProvider>
                  </GlobalPluginProvider>
                </ProjectEditProvider>
              </LocalLogProvider>
            </LogsProvider>
          </UserProvider>
        </ProjectRoutingProvider>
      </TeamProvider>
    </AuthProvider>
  </UserRoutingProvider>
</template>
