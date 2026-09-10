<script setup lang="ts">
  import { globalBillingRoutingSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { useGlobalBillingRouting } from '@/utils/hooks/useGlobalBillingRouting';

  const globalBillingRouting = useGlobalBillingRouting();
  provide<typeof globalBillingRouting>(
    globalBillingRoutingSymbol,
    globalBillingRouting,
  );
</script>

<template>
  <slot />
</template>
