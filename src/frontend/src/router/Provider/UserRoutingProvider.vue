<script setup lang="ts">
  import { useUserRouting } from '@/utils/hooks';
  import { userRoutingSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  const userRouting = useUserRouting();
  provide<typeof userRouting>(userRoutingSymbol, userRouting);
</script>

<template>
  <slot />
</template>
