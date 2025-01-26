<script lang="ts" setup>
  import { useUserStore, type UserStore } from '@/store';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  provide<UserStore>(userStoreSymbol, useUserStore());
</script>
<template>
  <slot />
</template>
