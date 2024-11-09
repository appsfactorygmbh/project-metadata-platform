<script lang="ts" setup>
  import { type ProjectsStore, projectStore } from '@/store';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  provide<ProjectsStore>(projectsStoreSymbol, projectStore);
</script>
<template>
  <slot></slot>
</template>
