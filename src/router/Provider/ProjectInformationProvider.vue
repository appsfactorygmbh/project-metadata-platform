<script lang="ts" setup>
  import { ProjectInformationStore } from '@/store/ProjectInformationStore';
  import { projectInformationStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';

  const projectInformationStore = ProjectInformationStore();
  provide<typeof projectInformationStore>(
    projectInformationStoreSymbol,
    projectInformationStore,
  );
</script>
<template>
  <slot></slot>
</template>
