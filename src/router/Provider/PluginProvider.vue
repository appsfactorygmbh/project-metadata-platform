<script setup lang="ts">
  import { ProjectsApi } from '@/api/generated';
  import { pluginService } from '@/services';
  import { usePluginsStore } from '@/store';
  import { pluginStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { useAuth } from 'vue-auth3';

  const pluginStore = usePluginsStore();
  provide<typeof pluginStore>(pluginStoreSymbol, pluginStore);

  const auth = useAuth();
  pluginService.initApi(auth.token(), ProjectsApi);
  watch(
    () => useAuth().token(),
    () => pluginService.initApi(auth.token(), ProjectsApi),
  );
</script>

<template>
  <slot></slot>
</template>
