<script lang="ts" setup>
  import { useProjectStore } from '@/store';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  const projectsStore = useProjectStore();
  provide<typeof projectsStore>(projectsStoreSymbol, projectsStore);
</script>
<template>
  <slot></slot>
</template>
