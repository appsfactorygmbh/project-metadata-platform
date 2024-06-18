<script lang="ts" setup>
  import { ProjectsStore } from '@/store/ProjectsStore';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  const projectsStore = ProjectsStore();
  provide<typeof projectsStore>(projectsStoreSymbol, projectsStore);
</script>
<template>
  <slot></slot>
</template>
