<script setup lang="ts">
  // import { useGlobalPluginsStore } from '@/store';
  import { globalPluginsStore } from '@/store/GlobalPluginStore';
  import { globalPluginStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  // const globalPluginsStore = useGlobalPluginsStore();
  provide<typeof globalPluginsStore>(
    globalPluginStoreSymbol,
    globalPluginsStore,
  );
</script>

<template>
  <slot></slot>
</template>
