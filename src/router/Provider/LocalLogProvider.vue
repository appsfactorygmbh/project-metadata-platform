<script setup lang="ts">
  import { localLogService } from '@/services';
  import { useLocalLogStore } from '@/store';
  import { localLogStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { useAuth } from 'vue-auth3';

  const localLogStore = useLocalLogStore();
  provide<typeof localLogStore>(localLogStoreSymbol, localLogStore);

  const auth = useAuth();
  localLogService.setAuth(auth?.token());
  watch(
    () => auth?.token(),
    () => localLogService.setAuth(auth.token()),
  );
</script>

<template>
  <slot></slot>
</template>
