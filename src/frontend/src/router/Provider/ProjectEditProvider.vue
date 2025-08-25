<script setup lang="ts">
  import { useProjectEditStore } from '@/store';
  import { projectEditStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  const projectEditStore = useProjectEditStore();
  provide<typeof projectEditStore>(projectEditStoreSymbol, projectEditStore);
</script>

<template>
  <slot />
</template>
