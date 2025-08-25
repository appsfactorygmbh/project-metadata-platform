<script setup lang="ts">
  import { useProjectRouting } from '@/utils/hooks';
  import { projectRoutingSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  const projectRouting = useProjectRouting();
  provide<typeof projectRouting>(projectRoutingSymbol, projectRouting);
</script>

<template>
  <slot />
</template>
