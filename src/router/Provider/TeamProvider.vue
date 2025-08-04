<script lang="ts" setup>
  import { useTeamStore, type TeamStore } from '@/store';
  import { teamStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  provide<TeamStore>(teamStoreSymbol, useTeamStore());
</script>
<template>
  <slot />
</template>
