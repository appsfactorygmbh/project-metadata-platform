<script setup lang="ts">
  import { teamRoutingSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { useTeamRouting } from '@/utils/hooks/useTeamRouting';

  const teamRouting = useTeamRouting();
  provide<typeof teamRouting>(teamRoutingSymbol, teamRouting);
</script>

<template>
  <slot />
</template>
