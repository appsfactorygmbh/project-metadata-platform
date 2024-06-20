<script setup lang="ts">
  import { usePluginsStore } from '@/store';
  import { pluginStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  const pluginStore = usePluginsStore();
  provide<typeof pluginStore>(pluginStoreSymbol, pluginStore);
</script>

<template>
  <slot></slot>
</template>
