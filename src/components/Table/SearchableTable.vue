<script lang="ts" setup>
  import { SmileOutlined, SearchOutlined } from '@ant-design/icons-vue';
  import { reactive, ref, watch, onMounted, inject, toRaw } from 'vue';
  import type { ComputedRef } from 'vue';
  import { useWindowSize } from '@vueuse/core';
  import type {
    FilterConfirmProps,
    FilterResetProps,
  } from 'ant-design-vue/es/table/interface';
  import type { ProjectModel } from '@/models/Project';
  import { storeToRefs } from 'pinia';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { useProjectStore } from '@/store/ProjectsStore';

  //Get the width of the left pane from App.vue
  const props = defineProps({
    paneWidth: {
      type: Number,
      required: true,
    },
    paneHeight: {
      type: Number,
      required: true,
    },
    isTest: {
      type: Boolean,
      default: false,
    },
  });

  //update paneWidth when the pane is resized
  watch(
    () => props.paneWidth,
    () => {
      changeColumns(props.paneWidth);
    },
  );

  const projectsStore = props.isTest
    ? useProjectStore()
    : inject(projectsStoreSymbol)!;

  const { getIsLoading } = storeToRefs(projectsStore);
  const isLoading = computed(() => getIsLoading.value);

  const customRow = (record: ProjectModel) => {
    return {
      onClick: () => {
        projectsStore.fetchProject(record.id);
      },
    };
  };

  onMounted(async () => {
    await projectsStore.fetchProjects();
    changeColumns(props.paneWidth);
    addTableEntry(projectsStore.getProjects);

    const data: ComputedRef<ProjectModel[]> = computed(
      () => projectsStore.getProjects,
    );

    // Updates Table, when a change in the store is detected
    watch(
      () => data.value,
      (newValue, oldValue) => {
        const newProject = newValue.filter(
          (newObj) => !oldValue.some((oldObj) => oldObj['id'] === newObj['id']),
        );
        addTableEntry(toRaw(newProject));
      },
    );
  });
</script>

<template>
  <!--
        Ant Design table with:
        columns: filtered if hidden or not
        scroll: sets height of table to ~90% of the window height
    -->
  <a-table
    :columns="[...columns].filter((item) => !item.hidden)"
    :data-source="[...dataSource]"
    :pagination="false"
    :loading="isLoading"
    :scroll="{ y: props.paneHeight - 55 }"
    :custom-row="customRow"
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

<script lang="ts">
  /*  Data implementation  */

  const dataSource: ProjectModel[] = reactive([]);

  /**
   * Adds a new table entry to dataSource.
   * @param {Project[]} data Stores the data that should be added.
   */
  function addTableEntry(data: ProjectModel[]) {
    for (const date of data) {
      dataSource.push({
        id: date.id,
        projectName: date.projectName,
        clientName: date.clientName,
        businessUnit: date.businessUnit,
        teamNumber: date.teamNumber,
      });
    }
  }

  /*  Column implementation  */

  /**
   * Adds an animation, when the box of the search element opens.
   * @param {boolean} visible True when the box is visible, False if not.
   */
  const filterDropdownAnimation = (visible: boolean) => {
    if (visible) {
      setTimeout(() => {
        searchInput.value.focus();
      }, 100);
    }
  };

  //sets the parameters for every column
  const columns = [
    {
      title: 'Project Name',
      dataIndex: 'projectName',
      key: 'projectName',
      customFilterDropdown: true,
      onFilter: (value: string | number | boolean, record: ProjectModel) =>
        record.projectName
          .toString()
          .toLowerCase()
          .includes(value.toString().toLowerCase()),
      onFilterDropdownOpenChange: (visible: boolean) => {
        filterDropdownAnimation(visible);
      },
      ellipsis: true,
      align: 'center' as const,
      sorter: (a: ProjectModel, b: ProjectModel) =>
        a.projectName.localeCompare(b.projectName),
      defaultSortOrder: 'ascend' as const,
    },
    {
      title: 'Client Name',
      dataIndex: 'clientName',
      key: 'clientName',
      customFilterDropdown: true,
      onFilter: (value: string | number | boolean, record: ProjectModel) =>
        record.clientName
          .toString()
          .toLowerCase()
          .includes(value.toString().toLowerCase()),
      onFilterDropdownOpenChange: (visible: boolean) => {
        filterDropdownAnimation(visible);
      },
      ellipsis: true,
      align: 'center' as const,
      sorter: (a: ProjectModel, b: ProjectModel) =>
        a.clientName.localeCompare(b.clientName),
      defaultSortOrder: 'ascend' as const,
      hidden: false,
    },
    {
      title: 'Business Unit',
      dataIndex: 'businessUnit',
      key: 'businessNumber',
      ellipsis: true,
      align: 'center' as const,
      sorter: (a: ProjectModel, b: ProjectModel) =>
        a.businessUnit.localeCompare(b.businessUnit),
      defaultSortOrder: 'ascend' as const,
      hidden: false,
    },
    {
      title: 'Team Number',
      dataIndex: 'teamNumber',
      key: 'teamNumber',
      ellipsis: true,
      align: 'center' as const,
      sorter: (a: ProjectModel, b: ProjectModel) => a.teamNumber - b.teamNumber,
      defaultSortOrder: 'ascend' as const,
      hidden: false,
    },
  ];

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

  /*  Column drop implementation  */

  /**
   * Changes the visible columns based on the width of the left pane.
   * @param {number} pwidth Has the width of the left pane.
   */
  function changeColumns(pwidth: number) {
    const breakpoint = getBreakpoint(pwidth);
    switch (breakpoint) {
      case 'xs':
        hideColumn('clientName');
        hideColumn('businessNumber');
        hideColumn('teamNumber');
        break;
      case 'sm':
        showColumn('clientName');
        hideColumn('businessNumber');
        hideColumn('teamNumber');
        break;
      case 'md':
        showColumn('clientName');
        showColumn('businessNumber');
        hideColumn('teamNumber');
        break;
      case 'lg':
        showColumn('clientName');
        showColumn('businessNumber');
        showColumn('teamNumber');
        break;
      default:
        hideColumn('clientName');
        hideColumn('businessNumber');
        hideColumn('teamNumber');
        break;
    }
  }

  /**
   * Hides given column.
   * @param {number} key Has the key of the column to hide.
   */
  function hideColumn(key: string) {
    columns.forEach((column) => {
      if (column.key == key) {
        column.hidden = true;
      }
    });
  }

  /**
   * Shows given column.
   * @param {number} key Has the key of the column to show.
   */
  function showColumn(key: string) {
    columns.forEach((column) => {
      if (column.key == key) {
        column.hidden = false;
      }
    });
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
      0.42 * windowSize,
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

<style>
  .highlight {
    background-color: rgb(255, 192, 105);
    padding: 0px;
  }
</style>
