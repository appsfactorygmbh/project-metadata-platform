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
  import ApiTokenRoutingProvider from './ApiTokenRoutingProvider.vue';
  import ApiTokenProvider from './ApiTokenProvider.vue';
</script>

<template>
  <UserRoutingProvider>
    <AuthProvider>
      <TeamProvider>
        <ApiTokenProvider>
          <ProjectRoutingProvider>
            <UserProvider>
              <LogsProvider>
                <LocalLogProvider>
                  <ProjectEditProvider>
                    <GlobalPluginProvider>
                      <TeamRoutingProvider>
                        <ApiTokenRoutingProvider>
                          <slot>
                            <RouterView />
                          </slot>
                        </ApiTokenRoutingProvider>
                      </TeamRoutingProvider>
                    </GlobalPluginProvider>
                  </ProjectEditProvider>
                </LocalLogProvider>
              </LogsProvider>
            </UserProvider>
          </ProjectRoutingProvider>
        </ApiTokenProvider>
      </TeamProvider>
    </AuthProvider>
  </UserRoutingProvider>
</template>
