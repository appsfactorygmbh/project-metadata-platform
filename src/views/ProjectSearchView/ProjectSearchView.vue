<script lang="ts" setup>
  import { SearchableTable } from '@/components/Table';
  import { ProjectsStore } from '@/store/ProjectsStore';
  import type { ProjectModel } from '@/models/ProjectModel';
  import { onMounted } from 'vue';

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

  const store = ProjectsStore();
  let tableData: ProjectModel[] = [];

  async function fetchProjects() {
    await store.fetchProjects();
    tableData = store.getProjects;
  }

  onMounted(fetchProjects);
</script>

<template>
  <SearchableTable
    :pane-width="props.paneWidth"
    :pane-height="props.paneHeight"
    :table-data="tableData"
  />
</template>
