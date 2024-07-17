<script setup lang="ts">
  import { pluginService } from '@/services';
  import { usePluginsStore } from '@/store';
  import { pluginStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { useAuth } from 'vue-auth3';

  const pluginStore = usePluginsStore();
  provide<typeof pluginStore>(pluginStoreSymbol, pluginStore);

  const auth = useAuth();
  pluginService.setAuth(auth.token());
  watch(
    () => useAuth().token(),
    () => pluginService.setAuth(auth.token()),
  );
</script>

<template>
  <slot></slot>
</template>
