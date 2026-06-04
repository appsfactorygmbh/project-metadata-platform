<script setup lang="ts">
  import { companyRoutingSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { useCompanyRouting } from '@/utils/hooks/useCompanyRouting';

  const companyRouting = useCompanyRouting();
  provide<typeof companyRouting>(companyRoutingSymbol, companyRouting);
</script>

<template>
  <slot />
</template>
