<script lang="ts" setup>
  import { useAuthorizationStore, type AuthorizationStore } from '@/store';
  import { authorizationStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  provide<AuthorizationStore>(
    authorizationStoreSymbol,
    useAuthorizationStore(),
  );
</script>
<template>
  <slot />
</template>
