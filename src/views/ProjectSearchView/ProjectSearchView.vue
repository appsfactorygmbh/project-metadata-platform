<script lang="ts" setup>
  import { SearchableTable } from '@/components/Table';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { onMounted, inject, provide } from 'vue';
  import { useSearchStore, type SearchStore } from '@/store/SearchStore';
  import type { ProjectModel } from '@/models/Project';

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
  const searchStore = useSearchStore<ProjectModel>('projects');
  const searchStoreSymbol = Symbol('projectSearchStore');

  provide<SearchStore>(searchStoreSymbol, searchStore);

  onMounted(async () => {
    await projectsStore.fetchProjects();
    searchStore.setBaseSet(projectsStore.projects);
  });
</script>

<template>
  <div style="padding: 20px">
    <a-flex vertical gap="middle">
      <SearchBar :search-store-symbol="searchStoreSymbol" />
      <SearchableTable
        :search-store-symbol="searchStoreSymbol"
        :pane-width="props.paneWidth"
        :pane-height="props.paneHeight"
      />
    </a-flex>
  </div>
</template>
