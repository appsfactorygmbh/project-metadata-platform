<script setup lang="ts">
  import { businessUnitRoutingSymbol } from '@/store/injectionSymbols';
  import { useBusinessUnitRouting } from '@/utils/hooks/useBusinessUnitRouting ';
  import { provide } from 'vue';

  const businessUnitRouting = useBusinessUnitRouting();
  provide<typeof businessUnitRouting>(
    businessUnitRoutingSymbol,
    businessUnitRouting,
  );
</script>

<template>
  <slot />
</template>
