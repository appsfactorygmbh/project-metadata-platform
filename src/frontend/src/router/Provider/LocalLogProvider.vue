<script setup lang="ts">
  import { useLocalLogStore, type LocalLogStore } from '@/store';
  import { localLogStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  provide<LocalLogStore>(localLogStoreSymbol, useLocalLogStore());
</script>

<template>
  <slot />
</template>
