<script setup lang="ts">
  import { useLogsStore, type LogsStore } from '@/store';
  import { logsStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  provide<LogsStore>(logsStoreSymbol, useLogsStore());
</script>

<template>
  <slot></slot>
</template>
