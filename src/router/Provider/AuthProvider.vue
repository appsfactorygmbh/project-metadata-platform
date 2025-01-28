<script setup lang="ts">
  import { authStoreSymbol } from '@/store/injectionSymbols';
  import { provide, watch } from 'vue';
  import { useAuth } from 'vue-auth3';
  import {
    useGlobalPluginsStore,
    useLocalLogStore,
    useLogsStore,
    useUserStore,
    useAuthStore,
    usePluginStore,
    useProjectStore
  } from '@/store';

  const router = useRouter();

  const authStore = useAuthStore();
  const projectStore = useProjectStore();
  const pluginStore = usePluginStore();
  const globalPluginStore = useGlobalPluginsStore();
  const localLogStore = useLocalLogStore();
  const logsStore = useLogsStore();
  const userStore = useUserStore();

  provide<typeof authStore>(authStoreSymbol, authStore);

  const auth = useAuth();

  authStore.setAuth(auth?.token() ?? null);
  globalPluginStore.refreshAuth();
  projectStore.refreshAuth();
  pluginStore.refreshAuth();
  localLogStore.refreshAuth();
  logsStore.refreshAuth();
  userStore.refreshAuth();

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
    auth?.load().then(() => {
      authInitialized.value = true;
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
          query: {
            redirect: router.currentRoute.value.path,
          },
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
    },
  );
</script>

<template>
  <slot />
</template>
