<script setup lang="ts">
  import {
    AuthProvider,
    GlobalPluginProvider,
    LocalLogProvider,
    ProjectEditProvider,
    UserProvider,
    TeamProvider,
    BusinessUnitRoutingProvider,
    DepartmentRoutingProvider,
    OfficeLocationRoutingProvider,
    CompanyRoutingProvider,
    DepartmentProvider,
    BusinessUnitProvider,
    OfficeLocationProvider,
    CompanyProvider,
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
          <CompanyProvider>
            <OfficeLocationProvider>
              <BusinessUnitProvider>
                <DepartmentProvider>
                  <ProjectRoutingProvider>
                    <UserProvider>
                      <LogsProvider>
                        <LocalLogProvider>
                          <ProjectEditProvider>
                            <GlobalPluginProvider>
                              <TeamRoutingProvider>
                                <ApiTokenRoutingProvider>
                                  <CompanyRoutingProvider>
                                    <OfficeLocationRoutingProvider>
                                      <BusinessUnitRoutingProvider>
                                        <DepartmentRoutingProvider>
                                          <slot>
                                            <RouterView />
                                          </slot>
                                        </DepartmentRoutingProvider>
                                      </BusinessUnitRoutingProvider>
                                    </OfficeLocationRoutingProvider>
                                  </CompanyRoutingProvider>
                                </ApiTokenRoutingProvider>
                              </TeamRoutingProvider>
                            </GlobalPluginProvider>
                          </ProjectEditProvider>
                        </LocalLogProvider>
                      </LogsProvider>
                    </UserProvider>
                  </ProjectRoutingProvider>
                </DepartmentProvider>
              </BusinessUnitProvider>
            </OfficeLocationProvider>
          </CompanyProvider>
        </ApiTokenProvider>
      </TeamProvider>
    </AuthProvider>
  </UserRoutingProvider>
</template>
