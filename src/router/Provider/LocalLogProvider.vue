<script setup lang="ts">
  import { logsService } from '@/services';
  import { useLocalLogStore } from '@/store';
  import { localLogStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { useAuth } from 'vue-auth3';

  const localLogStore = useLocalLogStore();
  provide<typeof localLogStore>(localLogStoreSymbol, localLogStore);

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
