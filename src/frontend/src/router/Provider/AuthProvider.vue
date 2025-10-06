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

  provide<typeof authStore>(authStoreSymbol, authStore);

  const auth = useAuth();
  var jwttoken = auth?.token()
  var ssotoken = await msalService.getAccessToken()
  authStore.setAuth(jwttoken ?? ssotoken, jwttoken != null ? "basic" : (ssotoken != null ? "oidc" : null) );
  globalPluginStore.refreshAuth();
  projectStore.refreshAuth();
  pluginStore.refreshAuth();
  localLogStore.refreshAuth();
  logsStore.refreshAuth();
  userStore.refreshAuth();
  teamStore.refreshAuth();

  const authInitialized = ref(false);
  const authenticationFailed = ref(false);
  const authenticated = ref(false);

  watch(
    () => auth?.check(),
    (check) => {
      if (authStore._authMethod == "basic") authenticated.value = check;
    },
  );

  watch(()=> msalService.getActiveUser(),
  (user) => {
    if (authStore._authMethod == "oicd") authenticated.value = !user==null

  }
)

  onMounted(() => {
if (authStore._authMethod == 'basic') {
      auth
        ?.load()
        .then(() => {
          authenticated.value = true;
          authInitialized.value = true;
        })
        .catch(() => {
          // Token refresh failed or initial load failed
          router.push('/login');
        });
} else {
    msalService.getAccessToken().then(() => {
        authenticated.value = true;
        authInitialized.value = true;
      })
      .catch(() => {
        // Token refresh failed or initial load failed
        router.push('/login');
      });
    }
  });

  watch(
    () => authInitialized.value && !authenticated.value,
    (initialized) => {
      if (!initialized) return;
      if (!auth?.check() && authStore.authMethod=="basic") {
        auth
          ?.refresh()
          .then(() => {
            authenticationFailed.value = false;
          })
          .catch(() => {
            authenticationFailed.value = true;
          });
      }
      if(msalService.getAccessToken == null && authStore.authMethod=="oidc"){
        authenticationFailed.value = true;
      }
    },
  );

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
    () => auth?.token(),
    (token) => {
      if (authStore._authMethod != 'oidc') {
        authStore.setAuth(token, "basic");
        globalPluginStore.refreshAuth();
        projectStore.refreshAuth();
        pluginStore.refreshAuth();
        localLogStore.refreshAuth();
        logsStore.refreshAuth();
        userStore.refreshAuth();
        teamStore.refreshAuth();
      }
    },
  );

    watch(
    () => msalService.getAccessToken(),
    async (token) => {
      if (authStore._authMethod != 'basic') {
        authStore.setAuth(await token, "oidc");
        globalPluginStore.refreshAuth();
        projectStore.refreshAuth();
        pluginStore.refreshAuth();
        localLogStore.refreshAuth();
        logsStore.refreshAuth();
        userStore.refreshAuth();
        teamStore.refreshAuth();
      }
    },
  );
</script>

<template>
  <slot />
</template>
