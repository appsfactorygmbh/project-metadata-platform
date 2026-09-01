<script setup lang="ts">
  import {
    AuthProvider,
    GlobalPluginProvider,
    LocalLogProvider,
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
  import AuthorizationProvider from './AuthorizationProvider.vue';
  import GlobalBillingProvider from './GlobalBillingProvider.vue';
  import GlobalBillingRoutingProvider from './GlobalBillingRoutingProvider.vue';
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
                          <GlobalBillingProvider>
                            <GlobalPluginProvider>
                              <TeamRoutingProvider>
                                <GlobalBillingRoutingProvider>
                                  <ApiTokenRoutingProvider>
                                    <CompanyRoutingProvider>
                                      <OfficeLocationRoutingProvider>
                                        <BusinessUnitRoutingProvider>
                                          <DepartmentRoutingProvider>
                                            <AuthorizationProvider>
                                              <slot>
                                                <RouterView />
                                              </slot>
                                            </AuthorizationProvider>
                                          </DepartmentRoutingProvider>
                                        </BusinessUnitRoutingProvider>
                                      </OfficeLocationRoutingProvider>
                                    </CompanyRoutingProvider>
                                  </ApiTokenRoutingProvider>
                                </GlobalBillingRoutingProvider>
                              </TeamRoutingProvider>
                            </GlobalPluginProvider>
                          </GlobalBillingProvider>
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
