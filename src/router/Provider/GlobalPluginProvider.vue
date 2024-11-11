<script setup lang="ts">
  import { type GlobalPluginsStore, useGlobalPluginsStore } from '@/store';
  import { globalPluginStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  // const globalPluginsStore = useGlobalPluginsStore();
  provide<GlobalPluginsStore>(globalPluginStoreSymbol, useGlobalPluginsStore());
</script>

<template>
  <slot></slot>
</template>
