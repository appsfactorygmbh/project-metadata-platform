<script setup lang="ts">
  import { type GlobalBillingStore, useGlobalBillingStore } from '@/store';
  import { globalBillingStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  provide<GlobalBillingStore>(
    globalBillingStoreSymbol,
    useGlobalBillingStore(),
  );
</script>

<template>
  <slot />
</template>
