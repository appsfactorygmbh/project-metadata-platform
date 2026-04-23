<script lang="ts" setup>
  import { type ApiTokenStore, useApiTokenStore } from '@/store';
  import { apiTokenStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  provide<ApiTokenStore>(apiTokenStoreSymbol, useApiTokenStore());
</script>
<template>
  <slot />
</template>
