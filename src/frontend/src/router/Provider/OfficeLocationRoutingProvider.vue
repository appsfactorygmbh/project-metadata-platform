<script setup lang="ts">
  import { officeLocationRoutingSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { useOfficeLocationRouting } from '@/utils/hooks/useOfficeLocationRouting';

  const officeLocationRouting = useOfficeLocationRouting();
  provide<typeof officeLocationRouting>(officeLocationRoutingSymbol, officeLocationRouting);
</script>

<template>
  <slot />
</template>
