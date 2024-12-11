<script lang="ts" setup>
  import { type SearchableColumn, SearchableTable } from '@/components/Table';
  import { SearchBar } from '@/components/Searchbar';
  import {
    projectRoutingSymbol,
    localLogStoreSymbol,
  } from '@/store/injectionSymbols';
  import { inject, onMounted, provide, reactive } from 'vue';
  import { type SearchStore, useSearchStore } from '@/store/SearchStore';
  import type { ProjectModel } from '@/models/Project';
  import { useEditing } from '@/utils/hooks/useEditing';
  import _ from 'lodash';
  import { useSessionStorage, useToggle, useWindowSize } from '@vueuse/core';
  import {
    BulbOutlined,
    InboxOutlined,
    UndoOutlined,
  } from '@ant-design/icons-vue';
  import { usePluginStore, useProjectStore } from '@/store';
  import { useQuery } from '@/utils/hooks';

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

  type ProjectSearchStore = SearchStore<ProjectModel>;

  const { stopEditing, isEditing } = useEditing();
  const { routerProjectId, setProjectId } = inject(projectRoutingSymbol)!;

  const localLogStore = inject(localLogStoreSymbol);
  const pluginStore = usePluginStore();
  const projectStore = useProjectStore();
  const searchStore = useSearchStore<ProjectModel>('projects');
  const searchStoreSymbol = Symbol('projectSearchStore');
  const isLoading = computed(() => projectStore.getIsLoadingProjects);
  provide<ProjectSearchStore>(searchStoreSymbol, searchStore);

  const searchQuery = useQuery(searchableColumnNames);
  const searchStorage = useSessionStorage('searchStorage', { searchQuery: '' });
  const filterStorage = useSessionStorage<Record<string, string>>(
    'filterStorage',
    {},
  );

  const showOnlyArchived: ProjectSearchStore['filter'] = (items) =>
    items.filter((item) => item.isArchived);
  const showOnlyActive: ProjectSearchStore['filter'] = (items) =>
    items.filter((item) => !item.isArchived);

  const [filterType, toggleFilterType] = useToggle<'active', 'archived'>(
    'active',
    {
      truthyValue: 'active',
      falsyValue: 'archived',
    },
  );
  const toggleShowFilter = () => {
    toggleFilterType();
    searchStore.setFilter(
      filterType.value === 'archived' ? showOnlyArchived : showOnlyActive,
    );
  };

  // on mount, set the filter to show only active projects
  searchStore.setFilter(showOnlyActive);

  watch(
    () => projectStore.getProjects,
    () => {
      searchStore.setBaseSet(projectStore.getProjects);
    },
  );

  const FETCHING_METHOD: 'FRONTEND' | 'BACKEND' = import.meta.env
    .VITE_PROJECT_SEARCH_METHOD;

  watch(
    () => projectStore.getProjects,
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
        return await projectStore.fetchAll({ search: value });
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
      await projectStore.fetch(routerProjectId.value);
      await pluginStore.fetch(routerProjectId.value);
      await pluginStore?.fetchUnarchived(routerProjectId.value);
      await localLogStore?.fetch(routerProjectId.value);
    },
  );

  const handleRowClick = async (project: ProjectModel) => {
    if (isEditing) await stopEditing();
    setProjectId(project.id);
  };

  const setFilterQuery = async () => {
    const filterKeys = Object.keys(filterStorage.value);
    for (const key of filterKeys) {
      if (filterStorage.value[key] === '') {
        await searchQuery.setSearchQuery(undefined, key);
      } else {
        await searchQuery.setSearchQuery(filterStorage.value[key], key);
      }
    }
  };

  onMounted(async () => {
    await projectStore.fetchAll();

    searchStore?.setSearchQuery(searchStorage.value.searchQuery);
    await setFilterQuery();

    if (routerProjectId.value === 0) {
      setProjectId(projectStore.getProjects[0]?.id ?? 100);
    } else {
      await projectStore.fetch(routerProjectId.value);
      await pluginStore.fetch(routerProjectId.value);
      await pluginStore?.fetchUnarchived(routerProjectId.value);
      await localLogStore?.fetch(routerProjectId.value);
    }

    searchStore.setBaseSet(projectStore.getProjects ?? []);
    changeColumns(props.paneWidth);
  });

  const clearAllFilters = () => {
    searchStore.reset();
    searchStore.applySearch();
  };
</script>

<template>
  <div style="padding: 20px">
    <a-flex vertical gap="middle">
      <span>
        <a-row :gutter="16" justify="space-between">
          <a-col :span="20">
            <SearchBar :search-store-symbol="searchStoreSymbol" width="100%" />
          </a-col>
          <a-col :span="2" style="display: flex; justify-content: flex-end">
            <a-tooltip
              placement="left"
              title="Click here to reset all filters"
              style="padding-left: 0; padding-right: 0"
            >
              <a-button
                style="width: 100%"
                name="resetButton"
                @click="clearAllFilters"
              >
                <template #icon>
                  <UndoOutlined class="icons" />
                </template>
              </a-button>
            </a-tooltip>
          </a-col>
          <a-col :span="2" style="display: flex; justify-content: flex-end">
            <a-tooltip
              placement="left"
              title="Click here to toggle between active and archived projects"
            >
              <a-button style="width: 100%" @click="toggleShowFilter">
                <template #icon>
                  <InboxOutlined v-if="filterType === 'active'" />
                  <BulbOutlined v-else />
                </template>
              </a-button>
            </a-tooltip>
          </a-col>
        </a-row>
      </span>

      <SearchableTable
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
  const searchableColumnNames = [
    'Project Name',
    'Client Name',
    'Business Unit',
    'Team Number',
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
<style scoped>
  .reset {
    position: absolute;
    top: 20px;
    right: 20px;
    width: 2.5em;
    height: 2.5em;
  }
</style>
