<script setup lang="ts">
  import { authStoreSymbol } from '@/store/injectionSymbols';
  import { provide, watch } from 'vue';
  import { useAuth } from 'vue-auth3';
  import {
    useGlobalPluginStore,
    useLocalLogStore,
    useLogsStore,
    useUserStore,
    useAuthStore,
    usePluginStore,
    useProjectStore,
    useTeamStore,
    useApiTokenStore,
    useCompanyStore,
    useDepartmentStore,
    useBusinessUnitStore,
    useOfficeLocationStore,
  } from '@/store';
  import { msalService } from '@/services/msalService';

  const router = useRouter();

  const authStore = useAuthStore();
  const projectStore = useProjectStore();
  const pluginStore = usePluginStore();
  const globalPluginStore = useGlobalPluginStore();
  const localLogStore = useLocalLogStore();
  const logsStore = useLogsStore();
  const userStore = useUserStore();
  const teamStore = useTeamStore();
  const companyStore = useCompanyStore();
  const departmentStore = useDepartmentStore();
  const businessUnitStore = useBusinessUnitStore();
  const officeLocationStore = useOfficeLocationStore();
  const apiTokenStore = useApiTokenStore();

  provide<typeof authStore>(authStoreSymbol, authStore);

  const auth = useAuth();
  const authInitialized = ref(false);
  const authenticationFailed = ref(false);
  const authenticated = ref(false);
  const refreshAllStores = () => {
    globalPluginStore.refreshAuth();
    projectStore.refreshAuth();
    pluginStore.refreshAuth();
    localLogStore.refreshAuth();
    logsStore.refreshAuth();
    userStore.refreshAuth();
    teamStore.refreshAuth();
    companyStore.refreshAuth();
    departmentStore.refreshAuth();
    businessUnitStore.refreshAuth();
    officeLocationStore.refreshAuth();
    apiTokenStore.refreshAuth();
  };
  onMounted(async () => {
    try {
      const jwttoken = auth?.token();
      const ssotoken = await msalService.getAccessTokenSilent();

      authStore.setAuth(
        jwttoken ?? ssotoken,
        jwttoken != null ? 'basic' : ssotoken != null ? 'oidc' : null,
      );

      refreshAllStores();

      if (authStore._authMethod == 'basic') {
        await auth?.load();
        authenticated.value = true;
      } else {
        if (ssotoken) {
          authenticated.value = true;
        } else {
          authenticationFailed.value = true;
        }
      }
    } catch (error) {
      console.warn('Auth initialization failed', error);
      authenticationFailed.value = true;
    } finally {
      authInitialized.value = true;
    }
  });

  watch(
    () => authenticationFailed.value,
    (failed) => {
      if (failed) {
        router.push({
          path: '/login',
        });
      }
    },
  );
  watch(
    () => auth?.check(),
    (check) => {
      if (authStore._authMethod == 'basic') authenticated.value = check;
    },
  );
</script>

<template>
  <template v-if="authInitialized">
    <slot />
  </template>
  <template v-else>
    <div style="display: flex; justify-content: center; padding: 2rem">
      Loading session...
    </div>
  </template>
</template>
