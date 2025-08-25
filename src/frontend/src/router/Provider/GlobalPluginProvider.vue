<script setup lang="ts">
  import { type GlobalPluginStore, useGlobalPluginStore } from '@/store';
  import { globalPluginStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  provide<GlobalPluginStore>(globalPluginStoreSymbol, useGlobalPluginStore());
</script>

<template>
  <slot />
</template>
