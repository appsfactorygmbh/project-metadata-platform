<script lang="ts" setup>
  import { SearchableTable, type SearchableColumn } from '@/components/Table';
  import {
    pluginStoreSymbol,
    projectsStoreSymbol,
  } from '@/store/injectionSymbols';
  import { onMounted, inject, provide, reactive } from 'vue';
  import { useSearchStore, type SearchStore } from '@/store/SearchStore';
  import type { ProjectModel } from '@/models/Project';
  import { projectsService } from '@/services';
  import _ from 'lodash';
  import { useWindowSize } from '@vueuse/core';
  import { useProjectRouting } from '@/utils/hooks';

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

  const { routerProjectId, setProjectId } = useProjectRouting();

  const projectsStore = inject(projectsStoreSymbol);
  const pluginStore = inject(pluginStoreSymbol);
  const searchStore = useSearchStore<ProjectModel>('projects');
  const searchStoreSymbol = Symbol('projectSearchStore');
  ``;
  const isLoading = computed(() => projectsStore?.getIsLoadingProjects);
  const searchableTable = ref<InstanceType<typeof SearchableTable>>();

  provide<SearchStore>(searchStoreSymbol, searchStore);

  const FETCHING_METHOD: 'FRONTEND' | 'BACKEND' = import.meta.env
    .VITE_PROJECT_SEARCH_METHOD;
  console.log('FETCHING_METHOD:', import.meta.env);

  watch(
    () => projectsStore?.getProjects,
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

  watch(
    () => routerProjectId.value,
    async () => {
      await projectsStore?.fetchProject(routerProjectId.value);
      await pluginStore?.fetchPlugins(routerProjectId.value);
    },
  );

  const handleRowClick = (project: ProjectModel) => {
    setProjectId(project.id);
  };

  onMounted(async () => {
    await projectsStore?.fetchProjects();

    if (routerProjectId.value === 0) {
      setProjectId(projectsStore?.getProjects[0]?.id || 100);
    } else {
      await projectsStore?.fetchProject(routerProjectId.value);
      await pluginStore?.fetchPlugins(routerProjectId.value);
    }

    searchStore.setBaseSet(projectsStore?.getProjects || []);
    changeColumns(props.paneWidth);
  });

  const clearAllFilters = () => {
    if (searchableTable.value && searchableTable.value.handleClearAll) {
      searchableTable.value.handleClearAll();
    }
    searchStore.reset();
  };
</script>

<template>
  <div style="padding: 20px">
    <a-flex vertical gap="middle">
      <SearchBar ref="SearchBar" :search-store-symbol="searchStoreSymbol" />
      <ResetButton @click="clearAllFilters" />

      <SearchableTable
        ref="searchableTable"
        :search-store-symbol="searchStoreSymbol"
        :pane-height="props.paneHeight"
        :columns="[...columns.filter((item) => !item.hidden)]"
        :is-loading="isLoading!"
        @row-click="handleRowClick"
      />
    </a-flex>
  </div>
</template>

<script lang="ts">
  //sets the parameters for every column
  const columns: SearchableColumn[] = reactive([
    {
      title: 'Project Name',
      dataIndex: 'projectName',
      key: 'projectName',
      searchable: true,
      ellipsis: true,
      align: 'center' as const,
      sortMethod: 'string',
      defaultSortOrder: 'ascend' as const,
      width: '37.5%',
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
      width: '37.5%',
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
      width: '12.5%',
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
      width: '12.5%',
    },
  ]);

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
      0.2 * windowSize,
      0.35 * windowSize,
      0.4 * windowSize,
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
