<script lang="ts" setup>
  import { SearchOutlined, SmileOutlined } from '@ant-design/icons-vue';
  import { inject, onMounted, reactive, ref } from 'vue';
  import type {
    FilterConfirmProps,
    FilterResetProps,
  } from 'ant-design-vue/es/table/interface';
  import type { SearchStore } from '@/store';
  import { numberSorter, stringSorter } from '@/utils/antd';
  import type { SearchableColumn } from './SearchableTableTypes';
  import type { TableColumnType, TableProps } from 'ant-design-vue';
  import type { ComputedRef, Ref } from 'vue';
  import type { ArrayElement } from '@/models/utils';
  import { useSessionStorage } from '@vueuse/core';
  import { useQuery, useThemeToken } from '@/utils/hooks';

  const token = useThemeToken();

  //Get the width of the left pane from App.vue
  const props = defineProps({
    searchStoreSymbol: {
      type: Symbol,
      required: true,
    },
    paneHeight: {
      type: Number,
      required: true,
    },
    columns: {
      type: Array<SearchableColumn>,
      required: true,
    },
    isLoading: {
      type: Boolean,
      required: true,
    },
  });

  interface FilteredInfoModel {
    [key: string]: string;
  }

  const searchStore = inject<SearchStore<object>>(props.searchStoreSymbol);
  const filterStorage = useSessionStorage<Record<string, string>>(
    'filterStorage',
    {},
  );
  const filteredInfo = reactive<FilteredInfoModel>({}); // Use an object to track filter values for each column

  const emit = defineEmits(['row-click']);

  const customRow = (record: object) => {
    return {
      onClick: () => {
        emit('row-click', record);
      },
    };
  };

  //saves the column dataIndexes that are searchable to create the useQuery hook with it
  const searchableColumnNames: Ref<string[]> = ref([]);
  props.columns.forEach((column) => {
    if (column.searchable) {
      searchableColumnNames.value.push(column.dataIndex);
    }
  });

  //reactive state for the search input of a column
  const searchInput = ref<HTMLInputElement>();

  const mapSearchableColumn = (
    column: ArrayElement<typeof props.columns>,
  ): TableColumnType => {
    const index = column.dataIndex;

    if (column.searchable) {
      column.onFilter = (value, record) =>
        String(record[index])
          .toLowerCase()
          .includes(value.toString().toLowerCase());
      column.customFilterDropdown = true;
      column.onFilterDropdownOpenChange = (visible: boolean) => {
        if (visible) {
          setTimeout(() => {
            searchInput.value?.focus();
          }, 100);
        }
      };
      // to set the coloumn.filteredValue to filtered.Info
      column.filteredValue = filteredInfo[index] ? [filteredInfo[index]] : null;
    }

    if (column.sortMethod) {
      if (column.sortMethod == 'string') {
        column.sorter = (a, b) => stringSorter(a, b, index);
      } else {
        column.sorter = (a, b) => numberSorter(a, b, index);
      }
    }
    return column;
  };

  const columns: ComputedRef<TableProps['columns']> = computed(() =>
    props.columns.map((column) => mapSearchableColumn(column)),
  );

  const setFilters = (filteredInfo: FilteredInfoModel) => {
    if (columns.value) {
      columns.value.forEach((column) => {
        const key = column.key;
        if (key) {
          column.filteredValue = filteredInfo[key] ? [filteredInfo[key]] : null;
          filterStorage.value = filteredInfo;
        }
      });
    }
  };

  // Watch filteredInfo to update columns' filteredValue reactively
  watch(filteredInfo, () => {
    setFilters(filteredInfo);
  });

  /*  Search implementation  */

  const { queryNames, routerSearchQuery, setSearchQuery } = useQuery(
    searchableColumnNames.value,
  );

  //saves state of searched text and in which column
  const state = reactive({
    searchText: '',
    searchedColumn: '',
  });

  /**
   * Saves the searched string and the target column in state, when search is confirmed.
   * @param {string} selectedKey Has the searched text in the first position.
   * @param {((param?: FilterConfirmProps) => void)} confirm Confirms the search.
   * @param {string} dataIndex Has the target column.
   */
  function handleSearch(
    selectedKey: string,
    confirm: (param?: FilterConfirmProps) => void,
    dataIndex: string,
  ) {
    confirm();
    setSearchQuery(selectedKey, dataIndex);

    state.searchText = selectedKey;
    state.searchedColumn = dataIndex;
    filteredInfo[dataIndex] = state.searchText;
  }

  /**
   * Function to check all columns before reset
   * @param {string | null} dataIndex String when resetting one column, null when resetting all columns.
   */
  async function resetSearchableColumns(dataIndex: string | null) {
    for (const columnName of searchableColumnNames.value) {
      if (!dataIndex || columnName === dataIndex) {
        filteredInfo[columnName] = '';
        await setSearchQuery(undefined, columnName);
      }
    }
  }

  /**
   * Resets the filtered search in target column.
   * @param {((param?: FilterResetProps) => void)} clearFilters Clears the filter, when confirmed.
   * @param {string} dataIndex Has the target column.
   */
  function handleReset(
    clearFilters: (param?: FilterResetProps) => void,
    dataIndex: string,
  ) {
    clearFilters({ confirm: true });
    resetSearchableColumns(dataIndex);
    state.searchText = '';
  }

  // Handle clear all filters action
  const handleClearAll = async () => {
    state.searchText = '';
    state.searchedColumn = '';
    await resetSearchableColumns(null);
  };
  searchStore?.setOnReset(handleClearAll);

  function handleResizeColumn(w: number, col: TableColumnType) {
    col.width = w;
  }

  onMounted(async () => {
    const filterKeys = Object.keys(filterStorage.value);
    filterKeys.forEach((key: string) => {
      filteredInfo[key] = filterStorage.value[key];
    });

    const queries = routerSearchQuery.value;

    for (const query in queries) {
      const searchQuery = queries[query];
      if (
        searchableColumnNames.value.includes(queryNames[query]) &&
        searchQuery !== 'undefined'
      ) {
        handleSearch(searchQuery as string, () => {}, queryNames[query]);
      }
    }
  });
</script>

<template>
  <!--
        Ant Design table with:
        columns: filtered if hidden or not
        scroll: sets height of table to ~90% of the window height
    -->
  <a-table
    class="clickable-table"
    size="small"
    :columns="[...columns]"
    :data-source="[...(searchStore?.getSearchResults || [])]"
    :pagination="false"
    :loading="isLoading"
    :scroll="{ y: props.paneHeight - 125, x: true }"
    :custom-row="customRow"
    :row-class-name="'table-row'"
    bordered
    @resize-column="handleResizeColumn"
  >
    <!-- Header of the table -->
    <template #headerCell="{ column }">
      <template v-if="column.key === 'name'">
        <span>
          <smile-outlined />
          Name
        </span>
      </template>
    </template>

    <!--
            Search function, when Search icon is clicked a box opens
            where you can input a string and filter the table with it
        -->
    <template
      #customFilterDropdown="{
        setSelectedKeys,
        selectedKeys,
        confirm,
        clearFilters,
        column,
      }"
    >
      <div style="padding: 8px">
        <!-- Input field for search, filters table from input when enter is pressed -->
        <a-input
          ref="searchInput"
          :placeholder="`Search ${column.dataIndex}`"
          :value="selectedKeys[0]"
          style="width: 188px; margin-bottom: 8px; display: block"
          @change="
            (e: any) => setSelectedKeys(e.target.value ? [e.target.value] : [])
          "
          @press-enter="
            handleSearch(selectedKeys[0], confirm, column.dataIndex)
          "
        />
        <!-- Search button, filters table from input when clicked -->
        <a-button
          type="primary"
          size="small"
          style="width: 90px; margin-right: 8px"
          @click="handleSearch(selectedKeys[0], confirm, column.dataIndex)"
        >
          <template #icon>
            <SearchOutlined />
          </template>
          Search
        </a-button>
        <!-- Reset button, resets filter when clicked -->
        <a-button
          size="small"
          style="width: 90px"
          @click="handleReset(clearFilters, column.dataIndex)"
        >
          Reset
        </a-button>
      </div>
    </template>

    <!-- Search icon, changes color when filter is applied -->
    <template #customFilterIcon="{ filtered }">
      <search-outlined :style="{ color: filtered ? '#108ee9' : undefined }" />
    </template>

    <!-- body of the table with all data entries -->
    <template #bodyCell="{ text, column }">
      <span
        v-if="state.searchText && state.searchedColumn === column.dataIndex"
      >
        {{ text }}
      </span>
    </template>
  </a-table>
</template>

<style scoped>
  .clickable-table :deep(.table-row) {
    cursor: pointer;
  }
  :deep(.ant-table-cell .ant-table-cell-ellipsis .ant-table-column-sort) {
    background-color: white;
  }
  :deep(.ant-table-expanded-row-fixed) {
    background-color: v-bind('token.colorBgElevated');
  }
</style>
