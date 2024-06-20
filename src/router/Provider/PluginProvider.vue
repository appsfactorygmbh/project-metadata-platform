<script setup lang="ts">
  import { usePluginsStore } from '@/store/Plugin/PluginStore.ts';
  import { pluginStoreSymbol } from '@/store/Plugin/injectionsSymbols.ts';
  import { provide } from 'vue';
  const pluginStore = usePluginsStore();
  provide<typeof pluginStore>(pluginStoreSymbol, pluginStore);
</script>

<template>
  <slot></slot>
</template>
