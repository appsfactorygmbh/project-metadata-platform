<script lang="ts" setup>
  import { SearchableTable, type SearchableColumn } from '@/components/Table';
  import {
    pluginStoreSymbol,
    projectsStoreSymbol,
  } from '@/store/injectionSymbols';
  import { onMounted, inject, provide } from 'vue';
  import { useSearchStore, type SearchStore } from '@/store/SearchStore';
  import type { ProjectModel } from '@/models/Project';
  import { projectsService } from '@/services';
  import _ from 'lodash';
  import { useWindowSize } from '@vueuse/core';

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

  const isLoading = computed(() => projectsStore?.getIsLoadingProjects);

  provide<SearchStore>(searchStoreSymbol, searchStore);

  watch(
    () => projectsStore.getProjects,
    () => {
      searchStore.setBaseSet(projectsStore.getProjects);
    },
  );

  const FETCHING_METHOD: 'FRONTEND' | 'BACKEND' = import.meta.env
    .VITE_PROJECT_SEARCH_METHOD;
  console.log('FETCHING_METHOD:', import.meta.env);

  watch(
    () => projectsStore.getProjects,
    (newData) => {
      searchStore?.setBaseSet(newData || []);
    },
  );

  //update paneWidth when the pane is resized
  watch(
    () => props.paneWidth,
    () => {
      changeColumns(props.paneWidth);
    },
  );

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

  const handleRowClick = (project: ProjectModel) => {
    projectsStore?.fetchProject(project.id);
  };

  onMounted(async () => {
    await projectsStore?.fetchProjects();
    const projectId = projectsStore?.getProjects[0].id || 100;

    await projectsStore?.fetchProject(projectId);
    await pluginStore?.fetchPlugins(projectId);
    searchStore.setBaseSet(projectsStore.getProjects);
    changeColumns(props.paneWidth);
  });
</script>

<template>
  <div style="padding: 20px">
    <a-flex vertical gap="middle">
      <SearchBar :search-store-symbol="searchStoreSymbol" />
      <SearchableTable
        :search-store-symbol="searchStoreSymbol"
        :pane-height="props.paneHeight"
        :columns="columns.filter((item) => !item.hidden)"
        :is-loading="isLoading"
        @row-click="handleRowClick"
      />
    </a-flex>
  </div>
</template>

<script lang="ts">
  //sets the parameters for every column
  const columns: SearchableColumn[] = [
    {
      title: 'Project Name',
      dataIndex: 'projectName',
      key: 'projectName',
      searchable: true,
      ellipsis: true,
      align: 'center' as const,
      sortMethod: 'string',
      defaultSortOrder: 'ascend' as const,
    },
    {
      title: 'Client Name',
      dataIndex: 'clientName',
      key: 'clientName',
      searchable: true,
      ellipsis: true,
      align: 'center' as const,
      sortMethod: 'string',
      defaultSortOrder: 'ascend' as const,
      hidden: false,
    },
    {
      title: 'Business Unit',
      dataIndex: 'businessUnit',
      key: 'businessNumber',
      ellipsis: true,
      align: 'center' as const,
      sortMethod: 'string',
      defaultSortOrder: 'ascend' as const,
      hidden: false,
    },
    {
      title: 'Team Number',
      dataIndex: 'teamNumber',
      key: 'teamNumber',
      ellipsis: true,
      align: 'center' as const,
      sortMethod: 'number',
      defaultSortOrder: 'ascend' as const,
      hidden: false,
    },
  ];

  /*  Column drop implementation  */

  /**
   * Changes the visible columns based on the width of the left pane.
   * @param {number} pwidth Has the width of the left pane.
   */
  function changeColumns(pwidth: number) {
    const breakpoint = getBreakpoint(pwidth);
    switch (breakpoint) {
      case 'xs':
        showOrHideColumns(0);
        break;
      case 'sm':
        showOrHideColumns(1);
        break;
      case 'md':
        showOrHideColumns(2);
        break;
      case 'lg':
        showOrHideColumns(3);
        break;
    }
  }

  /**
   * Shows the given amount of columns and hides the rest
   * @param number Has the number of how many columns should be shown
   */
  function showOrHideColumns(number: number) {
    for (let index: number = 1; index < 4; index++) {
      if (number > 0) {
        showColumn(index);
        number--;
      } else {
        hideColumn(index);
      }
    }
  }

  /**
   * Hides given column.
   * @param {number} index Has the index of the column to hide.
   */
  function hideColumn(index: number) {
    columns[index].hidden = true;
  }

  /**
   * Shows given column.
   * @param {number} index Has the index of the column to show.
   */
  function showColumn(index: number) {
    columns[index].hidden = false;
  }

  /**
   * Calculates the breakpoints based on the width of the window and assigns one based on the width of the left pane.
   * @param {number} pwidth Has the width of the left pane.
   * @return {string} Returns a string, which represents the current breakpoint of the pane width.
   */
  function getBreakpoint(pwidth: number): string {
    const windowSize = useWindowSize().width.value;
    const breakpoint: number[] = [
      0.25 * windowSize,
      0.37 * windowSize,
      0.5 * windowSize,
    ];

    if (pwidth > breakpoint[2]) {
      return 'lg';
    } else if (pwidth > breakpoint[1]) {
      return 'md';
    } else if (pwidth > breakpoint[0]) {
      return 'sm';
    } else {
      return 'xs';
    }
  }
</script>
