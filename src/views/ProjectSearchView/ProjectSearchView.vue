<script lang="ts" setup>
  import { SearchableTable } from '@/components/Table';
  import {
    pluginStoreSymbol,
    projectsStoreSymbol,
  } from '@/store/injectionSymbols';
  import { onMounted, inject, provide } from 'vue';
  import { useSearchStore, type SearchStore } from '@/store/SearchStore';
  import type { ProjectModel } from '@/models/Project';
  import { projectsService } from '@/services';
  import _ from 'lodash';

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
  const pluginStore = inject(pluginStoreSymbol);
  const searchStore = useSearchStore<ProjectModel>('projects');
  const searchStoreSymbol = Symbol('projectSearchStore');

  provide<SearchStore>(searchStoreSymbol, searchStore);

  const FETCHING_METHOD: 'FRONTEND' | 'BACKEND' = import.meta.env
    .VITE_PROJECT_SEARCH_METHOD;
  console.log('FETCHING_METHOD:', import.meta.env);

  if (FETCHING_METHOD === 'BACKEND') {
    const fetchData = async (value: string) => {
      try {
        return await projectsService.fetchProjects(value);
      } catch (error) {
        console.error('Failed to fetch data:', error);
      }
    };

    // Debounced version of fetchData
    const debouncedFetchData = _.debounce(fetchData, 300);

    // Input Listener
    watch(
      () => searchStore.getSearchQuery,
      () => {
        debouncedFetchData(searchStore.getSearchQuery)?.then((data) => {
          searchStore?.setBaseSet(data || []);
        });
        searchStore?.setSearchQuery('');
      },
    );
  }

  onMounted(async () => {
    await projectsStore?.fetchProjects();
    const projectId = projectsStore?.getProjects[0].id || 100;

    await projectsStore?.fetchProject(projectId);
    await pluginStore?.fetchPlugins(projectId);
    searchStore.setBaseSet(projectsStore.getProjects);
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
