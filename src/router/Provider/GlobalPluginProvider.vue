<script setup lang="ts">
  import { useGlobalPluginsStore } from '@/store';
  import { globalPluginStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { globalPluginService } from '@/services';
  import { useAuth } from 'vue-auth3';

  const globalPluginsStore = useGlobalPluginsStore();
  provide<typeof globalPluginsStore>(
    globalPluginStoreSymbol,
    globalPluginsStore,
  );

  const auth = useAuth();
  globalPluginService.setAuth(auth.token());

  watch(
    () => auth.token(),
    () => globalPluginService.setAuth(auth.token()),
  );
</script>

<template>
  <slot></slot>
</template>
