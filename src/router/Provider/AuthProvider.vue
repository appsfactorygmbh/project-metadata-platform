<script setup lang="ts">
  import { authStoreSymbol } from '@/store/injectionSymbols';
  import { provide, watch } from 'vue';
  import { useAuth } from 'vue-auth3';
  import {
    useGlobalPluginsStore,
    useLocalLogStore,
    useLogsStore,
  } from '@/store';
  import { useAuthStore, usePluginStore, useProjectStore } from '@/store';

  const authStore = useAuthStore();
  const projectStore = useProjectStore();
  const pluginStore = usePluginStore();
  const globalPluginStore = useGlobalPluginsStore();
  const localLogStore = useLocalLogStore();
  const logsStore = useLogsStore();

  provide<typeof authStore>(authStoreSymbol, authStore);

  const auth = useAuth();

  authStore.setAuth(auth?.token() ?? null);
  globalPluginStore.refreshAuth();
  projectStore.refreshAuth();
  pluginStore.refreshAuth();
  localLogStore.refreshAuth();
  logsStore.refreshAuth();

  watch(
    () => auth?.token(),
    (token) => {
      authStore.setAuth(token ?? null);
      globalPluginStore.refreshAuth();
      projectStore.refreshAuth();
      pluginStore.refreshAuth();
      localLogStore.refreshAuth();
      logsStore.refreshAuth();
    },
  );
</script>

<template>
  <slot></slot>
</template>
