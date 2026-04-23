<script setup lang="ts">
  import { apiTokenRoutingSymbol } from '@/store/injectionSymbols';
  import { useApiTokenRouting } from '@/utils/hooks';
  import { provide } from 'vue';

  const apiTokenRouting = useApiTokenRouting();
  provide<typeof apiTokenRouting>(apiTokenRoutingSymbol, apiTokenRouting);
</script>

<template>
  <slot />
</template>
