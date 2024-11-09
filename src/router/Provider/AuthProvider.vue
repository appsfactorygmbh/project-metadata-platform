<script setup lang="ts">
  import { authStoreSymbol } from '@/store/injectionSymbols';
  import { provide, watch } from 'vue';
  import { useAuth } from 'vue-auth3';
  import { authStore } from '@/store/AuthStore';
  import { globalPluginsStore } from '@/store/GlobalPluginStore';

  provide<typeof authStore>(authStoreSymbol, authStore);

  const auth = useAuth();
  authStore.setAuth(auth.token());
  globalPluginsStore.refreshAuth();

  watch(
    () => auth.token(),
    () => {
      authStore.setAuth(auth.token());
      globalPluginsStore.refreshAuth();
    },
  );
</script>

<template>
  <slot></slot>
</template>
