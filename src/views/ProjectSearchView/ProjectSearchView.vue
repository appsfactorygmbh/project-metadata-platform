<script lang="ts" setup>
  import { SearchableTable } from '@/components/Table';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { onMounted, inject } from 'vue';

  const props = defineProps({
    paneWidth: {
      type: Number,
      required: true,
    },
    paneHeight: {
      type: Number,
      required: true,
    },
  });

  const projectsStore = inject(projectsStoreSymbol)!;

  onMounted(async () => {
    await projectsStore.fetchProjects();
  });
</script>

<template>
  <SearchableTable
    :pane-width="props.paneWidth"
    :pane-height="props.paneHeight"
  />
</template>
