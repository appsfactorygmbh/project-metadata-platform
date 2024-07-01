<script lang="ts" setup>
  import { SmileOutlined, SearchOutlined } from '@ant-design/icons-vue';
  import { reactive, ref, inject } from 'vue';
  import type {
    FilterConfirmProps,
    FilterResetProps,
  } from 'ant-design-vue/es/table/interface';
  import type { ProjectModel } from '@/models/Project';
  import type { SearchStore } from '@/store';
  import type {
    SearchableColumn,
    SearchableColumns,
  } from './SearchableTableTypes';

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

  const searchStore = inject<SearchStore<ProjectModel>>(
    props.searchStoreSymbol,
  );

  const emit = defineEmits(['row-click']);

  const customRow = (record: ProjectModel) => {
    return {
      onClick: () => {
        emit('row-click', record.id);
      },
    };
  };

  const searchableColumn: SearchableColumns = props.columns;
  searchableColumn.forEach((column) => {
    if (column.searchable) {
      column.onFilter = (
        value: string | number | boolean,
        record: ProjectModel,
      ) =>
        record.projectName
          .toString()
          .toLowerCase()
          .includes(value.toString().toLowerCase());
      column.customFilterDropdown = true;
      column.onFilterDropdownOpenChange = (visible: boolean) => {
        if (visible) {
          setTimeout(() => {
            searchInput.value.focus();
          }, 100);
        }
      };
    }
    if (column.sortMethod) {
      if (column.sortMethod == 'string') {
        column.sorter = (a: ProjectModel, b: ProjectModel) =>
          a.projectName.localeCompare(b.projectName);
      } else {
        column.sorter = (a: ProjectModel, b: ProjectModel) =>
          a.teamNumber - b.teamNumber;
      }
    }
  });

  /*  Search implementation  */

  //saves state of searched text and in which column
  const state = reactive({
    searchText: '',
    searchedColumn: '',
  });

  const searchInput = ref();

  /**
   * Saves the searched string and the target column in state, when search is confirmed.
   * @param {string[]} selectedKeys Has the searched text in the first position.
   * @param {((param?: FilterConfirmProps) => void)} confirm Confirms the search.
   * @param {string} dataIndex Has the target column.
   */
  function handleSearch(
    selectedKeys: string[],
    confirm: (param?: FilterConfirmProps) => void,
    dataIndex: string,
  ) {
    confirm();
    state.searchText = selectedKeys[0];
    state.searchedColumn = dataIndex;
  }

  /**
   * Resets the filtered search in target column.
   * @param {((param?: FilterResetProps) => void)} clearFilters Clears the filter, when confirmed.
   */
  function handleReset(clearFilters: (param?: FilterResetProps) => void) {
    clearFilters({ confirm: true });
    state.searchText = '';
  }
</script>

<template>
  <!--
        Ant Design table with:
        columns: filtered if hidden or not
        scroll: sets height of table to ~90% of the window height
    -->
  <a-table
    class="clickable-table"
    :columns="[...props.columns]"
    :data-source="[...(searchStore?.getSearchResults || [])]"
    :pagination="false"
    :loading="props.isLoading"
    :scroll="{ y: props.paneHeight - 155 }"
    :custom-row="customRow"
    :row-class-name="'row'"
    bordered
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
          @press-enter="handleSearch(selectedKeys, confirm, column.dataIndex)"
        />
        <!-- Search button, filters table from input when clicked -->
        <a-button
          type="primary"
          size="small"
          style="width: 90px; margin-right: 8px"
          @click="handleSearch(selectedKeys, confirm, column.dataIndex)"
        >
          <template #icon><SearchOutlined /></template>
          Search
        </a-button>
        <!-- Reset button, resets filter when clicked -->
        <a-button
          size="small"
          style="width: 90px"
          @click="handleReset(clearFilters)"
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
      <!-- span only shows when the specific column is searched on-->
      <span
        v-if="state.searchText && state.searchedColumn === column.dataIndex"
      >
        <!-- splits the string from the data entries, so i can be highlighted -->
        <template
          v-for="(fragment, i) in text
            .toString()
            .split(
              new RegExp(
                `(?<=${state.searchText})|(?=${state.searchText})`,
                'i',
              ),
            )"
        >
          <!-- string that is searched gets highlighted in the table entries-->
          <mark
            v-if="fragment.toLowerCase() === state.searchText.toLowerCase()"
            :key="i"
            class="highlight"
          >
            {{ fragment }}
          </mark>
          <template v-else>{{ fragment }}</template>
        </template>
      </span>
    </template>
  </a-table>
</template>

<style scoped>
  .highlight {
    background-color: rgb(255, 192, 105);
    padding: 0px;
  }

  .clickable-table :deep(.row) {
    cursor: pointer;
  }
</style>
