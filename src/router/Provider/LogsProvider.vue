<script setup lang="ts">
  import { logsService } from '@/services';
  import { useLogsStore } from '@/store';
  import { logsStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { useAuth } from 'vue-auth3';

  const logsStore = useLogsStore();
  provide<typeof logsStore>(logsStoreSymbol, logsStore);

  const auth = useAuth();
  logsService.setAuth(auth?.token());
  watch(
    () => auth?.token(),
    () => logsService.setAuth(auth.token()),
  );
</script>

<template>
  <slot></slot>
</template>
