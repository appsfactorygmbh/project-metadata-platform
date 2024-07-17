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
  watch(
    () => auth.currentToken,
    () => globalPluginService.setAuth(auth.currentToken),
  );
</script>

<template>
  <slot></slot>
</template>
