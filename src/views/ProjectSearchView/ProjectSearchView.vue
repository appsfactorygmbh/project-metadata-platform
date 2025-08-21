<script lang="ts" setup>
  import { type SearchableColumn, SearchableTable } from '@/components/Table';
  import { SearchBar } from '@/components/Searchbar';
  import {
    projectRoutingSymbol,
    localLogStoreSymbol,
  } from '@/store/injectionSymbols';
  import { inject, onMounted, provide, reactive } from 'vue';
  import { type SearchStore, useSearchStore } from '@/store/SearchStore';
  import type { ProjectModel, ProjectSearchModel } from '@/models/Project';
  import { useEditing } from '@/utils/hooks/useEditing';
  import _ from 'lodash';
  import { useSessionStorage, useToggle, useWindowSize } from '@vueuse/core';
  import {
    BulbOutlined,
    InboxOutlined,
    UndoOutlined,
  } from '@ant-design/icons-vue';
  import { usePluginStore, useProjectStore } from '@/store';
  import { useQuery, useThemeToken } from '@/utils/hooks';

  const token = useThemeToken();

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

  type ProjectSearchStore = SearchStore<ProjectSearchModel>;

  const { stopEditing, isEditing } = useEditing();
  const { routerProjectId, routerProjectSlug, setProjectId } =
    inject(projectRoutingSymbol)!;
  const localLogStore = inject(localLogStoreSymbol);

  const pluginStore = usePluginStore();
  const projectStore = useProjectStore();
  const searchStore = useSearchStore<ProjectSearchModel>('projects');
  const searchStoreSymbol = Symbol('projectSearchStore');
  const isLoading = computed(() => projectStore.getIsLoadingProjects);

  const toSearchModelConverter = (model: ProjectModel) => {
    return {
      id: model.id,
      slug: model.slug,
      projectName: model.projectName,
      clientName: model.clientName,
      company: model.company,
      isArchived: model.isArchived,
      ismsLevel: model.ismsLevel,
      teamName: model.team === undefined ? '' : model.team.teamName,
      businessUnit: model.team === undefined ? '' : model.team.businessUnit,
    } as ProjectSearchModel;
  };

  provide<ProjectSearchStore>(searchStoreSymbol, searchStore);

  const searchQuery = useQuery(queryNames);
  const searchStorage = useSessionStorage('searchStorage', { searchQuery: '' });
  const filterStorage = useSessionStorage<Record<string, string>>(
    'filterStorage',
    {},
  );

  const highlightButtonStyle = computed(() =>
    searchQuery.isSearchQuery.value
      ? { color: '#3e8ee2', width: '100%', borderColor: '#3e8ee2' }
      : {
          color: token.value.colorText,
          width: '100%',
        },
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
  const toggleShowFilter = async () => {
    toggleFilterType();
    searchStore.setFilter(
      filterType.value === 'archived' ? showOnlyArchived : showOnlyActive,
    );
    if (isEditing) await stopEditing();
    setProjectId(undefined);

  };

  // on mount, set the filter to show only active projects
  onMounted(() => searchStore.setFilter(showOnlyActive));

  const FETCHING_METHOD: 'FRONTEND' | 'BACKEND' = import.meta.env
    .VITE_PROJECT_SEARCH_METHOD;

  watch(
    () => projectStore.getProjects,
    (newData) => {
      searchStore?.setBaseSet(newData.map(toSearchModelConverter) || []);
      if (newData.length === 0) {
        searchStore?.applySearch();
      }
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
          searchStore?.setBaseSet(data?.map(toSearchModelConverter) ?? []);
        });
        searchStore?.setSearchQuery('');
      },
    );
  }

  watch(
    () => routerProjectId.value,
    async () => {
      if (!routerProjectId.value) return;
      await fetchProject(routerProjectId.value);
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

  const fetchProject = async (id: number) => {
    await projectStore.fetch(id);
    await pluginStore.fetch(id);
    await pluginStore?.fetchUnarchived(id);
    await localLogStore?.fetch(id);
  };

  onMounted(async () => {
    await projectStore.fetchAll();

    searchStore?.setSearchQuery(searchStorage.value.searchQuery);
    await setFilterQuery();
    console.log(routerProjectSlug.value, routerProjectId.value);

    if (routerProjectSlug.value !== undefined) {
      const project = await projectStore.findProjectBySlug(
        routerProjectSlug.value,
        { fullObjectNeeded: false },
      );
      if (project) {
        if (routerProjectId.value === project.id) fetchProject(project.id);
        setProjectId(project.id);
      }
    } else if (routerProjectId.value) {
      await fetchProject(routerProjectId.value);
    }

    searchStore.setBaseSet(
      projectStore.getProjects.map(toSearchModelConverter) ?? [],
    );
    changeColumns(props.paneWidth);
  });

  const clearAllFilters = () => {
    searchStore.reset();
    searchStorage.value.searchQuery = '';
    searchStore.applySearch();
  };
</script>

<template>
  <div style="padding: 10px">
    <a-flex vertical gap="middle">
      <span style="display: flex; flex-direction: row">
        <SearchBar :search-store-symbol="searchStoreSymbol" style="flex: 5" />
        <a-tooltip
          placement="left"
          title="Click here to reset all filters"
          style="padding-left: 0; padding-right: 0"
        >
          <a-button
            class="button"
            name="resetButton"
            :style="highlightButtonStyle"
            @click="clearAllFilters"
          >
            <template #icon>
              <UndoOutlined class="icons" />
            </template>
          </a-button>
        </a-tooltip>
        <a-tooltip
          placement="left"
          title="Click here to toggle between active and archived projects"
        >
          <a-button class="button" @click="toggleShowFilter">
            <template #icon>
              <InboxOutlined v-if="filterType === 'active'" />
              <BulbOutlined v-else />
            </template>
          </a-button>
        </a-tooltip>
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
      title: 'Client Name',
      dataIndex: 'clientName',
      key: 'clientName',
      searchable: true,
      resizable: true,
      ellipsis: true,
      align: 'center' as const,
      sortMethod: 'string',
      defaultSortOrder: 'ascend' as const,
      width: NaN,
    },
    {
      title: 'Project Name',
      dataIndex: 'projectName',
      key: 'projectName',
      searchable: true,
      resizable: true,
      ellipsis: true,
      align: 'center' as const,
      sortMethod: 'string',
      defaultSortOrder: 'ascend' as const,
      width: NaN,
      hidden: false,
      hasTags: true,
      getTagColor: (record): string =>
        record.ismsLevel === 'VERY_HIGH'
          ? 'red'
          : record.ismsLevel === 'HIGH'
            ? 'orange'
            : 'green',
    },
    {
      title: 'Company',
      dataIndex: 'company',
      key: 'company',
      searchable: true,
      resizable: true,
      ellipsis: true,
      align: 'center' as const,
      sortMethod: 'string',
      defaultSortOrder: 'ascend' as const,
      hidden: false,
      width: NaN,
    },
    {
      title: 'Business Unit',
      dataIndex: 'businessUnit',
      key: 'businessUnit',
      searchable: true,
      resizable: true,
      ellipsis: true,
      align: 'center' as const,
      sortMethod: 'string',
      defaultSortOrder: 'ascend' as const,
      hidden: false,
      width: NaN,
    },
    {
      title: 'Team Name',
      dataIndex: 'teamName',
      key: 'teamName',
      searchable: true,
      ellipsis: true,
      align: 'center' as const,
      sortMethod: 'string',
      defaultSortOrder: 'ascend' as const,
      hidden: false,
    },
  ]);
  const queryNames = [
    'searchQuery',
    'clientName',
    'projectName',
    'company',
    'businessUnit',
    'teamNumber',
  ];

  /*  Column drop implementation  */

  /**
   * Changes the visible columns based on the width of the left pane.
   * @param {number} pwidth Has the width of the left pane.
   */
  function changeColumns(pwidth: number) {
    // getBreakpoint returns the index of the breakpoint, which is one less then the index of the column to hide/show
    const columnToChange = getBreakpoint(pwidth) + 1;

    if (columnToChange >= 0) showOrHideColumns(columnToChange);
  }

  /**
   * Shows the given amount of columns and hides the rest
   * @param number Has the number of how many columns should be shown
   */
  function showOrHideColumns(number: number) {
    for (let index: number = 1; index < columns.length; index++) {
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
    columns[index - 1].resizable = false;
    delete columns[index - 1].width;
  }

  /**
   * Shows given column.
   * @param {number} index Has the index of the column to show.
   */
  function showColumn(index: number) {
    columns[index].hidden = false;
    columns[index - 1].resizable = true;
    columns[index - 1].width = NaN;
  }

  /**
   * Calculates the breakpoints based on the width of the window and assigns one based on the width of the left pane.
   * @param {number} pwidth Has the width of the left pane.
   * @return {string} Returns a string, which represents the current breakpoint of the pane width.
   */
  function getBreakpoint(pwidth: number): number {
    const windowSize = useWindowSize().width.value;
    // has the breakpoints for the columns
    // smaller then the first one and only one column is shown, larger then the last one and all are shown
    const breakpoints: number[] = [
      0.2 * windowSize,
      0.275 * windowSize,
      0.35 * windowSize,
      0.4 * windowSize,
    ];

    // runs backwards through the breakpoints and checks if the width is larger then the breakpoint
    for (let index = breakpoints.length - 1; index >= 0; index--) {
      if (pwidth > breakpoints[index]) {
        return index;
      }
    }

    return -1;
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

  .button {
    flex: 1;
    width: 100%;
    max-width: 5em;
    min-width: 2.5em;
    margin-left: 0.5em;
  }
</style>
