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

  authStore.setAuth(auth?.token() ?? null);
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
      authenticated.value = check;
    },
  );

  onMounted(() => {
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
  });

  watch(
    () => authInitialized.value && !authenticated.value,
    (initialized) => {
      if (!initialized) return;
      if (!auth?.check()) {
        auth
          ?.refresh()
          .then(() => {
            authenticationFailed.value = false;
          })
          .catch(() => {
            console.log('refresh failed');
            authenticationFailed.value = true;
          });
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
      authStore.setAuth(token ?? null);
      globalPluginStore.refreshAuth();
      projectStore.refreshAuth();
      pluginStore.refreshAuth();
      localLogStore.refreshAuth();
      logsStore.refreshAuth();
      userStore.refreshAuth();
      teamStore.refreshAuth();
    },
  );
</script>

<template>
  <slot />
</template>
