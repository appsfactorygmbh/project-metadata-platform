<script lang="ts" setup>
    import { SmileOutlined, SearchOutlined } from "@ant-design/icons-vue";
    import { reactive, ref, watch, onMounted } from "vue";
    import { useWindowSize } from "@vueuse/core";
    import { FilterConfirmProps, FilterResetProps } from "ant-design-vue/es/table/interface";

    //Get the width of the left pane from App.vue
    const props = defineProps({
        paneWidth: {
            type: Number,
            required: true
        },
        paneHeight: {
            type: Number,
            required: true
        },
        isTest: {
            type: Boolean,
            required: true
        }
    });

    //update paneWidth when the pane is resized
    watch(
        () => props.paneWidth,
        () => {
            changeColumns(props.paneWidth);
        }
    )
    
    /**
     * Fetches the API (for showcase local file) and adds every data entry into the data source
     */
    const fetchData = async () => {
        try {
            const response = await fetch(
                "./src/components/Table/testData.json"
            )

            const data: Data[] = await response.json();            
            
            addTableEntry(data);

        } catch(err) {
            console.error("Error fetching data: " + err);   
        }
    }

    /**
     * Fetches the data or loads test data if its a test
     */
    function loadData (isTest: boolean) {
        if (isTest) {
            addTableEntry(testData);
        } else {
            fetchData();
        }
    }
    
    //calculates current window height for the scroll value
    const windowHeight = ref(useWindowSize().height.value);

    onMounted( () => {      
        loadData(props.isTest);
        changeColumns(props.paneWidth);
    });
    
</script>

<template>
    <!-- 
        Ant Design table with: 
        columns: filtered if hidden or not
        scroll: sets height of table to 900 and activates scrolling
    -->
    <a-table
        :columns="[...columns].filter(item => !item.hidden)" 
        :data-source="[...dataSource]" 
        :pagination="false"
        :scroll="{ y: 0.904*windowHeight }"
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
            #customFilterDropdown="{ setSelectedKeys, selectedKeys, confirm, clearFilters, column }"
        >
            <div style="padding: 8px">
                <!-- Input field for search, filters table from input when enter is pressed -->
                <a-input
                    ref="searchInput"
                    :placeholder="`Search ${column.dataIndex}`"
                    :value="selectedKeys[0]"
                    style="width: 188px; margin-bottom: 8px; display: block"
                    @change="e => setSelectedKeys(e.target.value ? [e.target.value] : [])"
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
                <a-button size="small" style="width: 90px" @click="handleReset(clearFilters)">
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
            <span v-if="state.searchText && state.searchedColumn === column.dataIndex">
                <!-- splits the string from the data entries, so i can be highlighted -->
                <template
                    v-for="(fragment, i) in text
                        .toString()
                        .split(new RegExp(`(?<=${state.searchText})|(?=${state.searchText})`, 'i'))"
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

type Data = {
    ProjectName: string;
    ClientName: string;
    BusinessUnit: string;
    TeamNumber: number;
}

type DataItem = {
    key: number;
    pname: string;
    cname: string;
    bu: string;
    tnr: number;
}

const dataSource: DataItem[] = reactive([]);

/**
 * Adds a new table entry to dataSource.
 * @param {Data[]} data Stores the data that should be added.
 */
function addTableEntry(data: Data[]) {
    for (const date of data) {       
        dataSource.push({
            key: dataSource.length + 1,
            pname: date.ProjectName,
            cname: date.ClientName,
            bu: date.BusinessUnit,
            tnr: date.TeamNumber
        });
    }
}

const testData = [
    {
        ProjectName: "C",
        ClientName: "A",
        BusinessUnit: "A",
        TeamNumber: 1
    },
    {
        ProjectName: "A",
        ClientName: "B",
        BusinessUnit: "B",
        TeamNumber: 2
    },
    {
        ProjectName: "B",
        ClientName: "C",
        BusinessUnit: "C",
        TeamNumber: 3
    }
];

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
        title: "Project Name",
        dataIndex: "pname",
        key: "pname",
        customFilterDropdown: true,
        onFilter: (value: string | number | boolean, record: any) => 
            record.pname.toString().toLowerCase().includes(value.toString().toLowerCase()),
        onFilterDropdownOpenChange: (visible: boolean) => { filterDropdownAnimation(visible) },
        ellipsis: true,
        align: "center" as const,
        sorter: (a: DataItem, b: DataItem) => a.pname.localeCompare(b.pname),
        defaultSortOrder: "ascend" as const,
        hidden: false
    },
    {
        title: "Client Name",
        dataIndex: "cname",
        key: "cname",
        customFilterDropdown: true,
        onFilter: (value: string | number | boolean, record: any) => 
            record.cname.toString().toLowerCase().includes(value.toString().toLowerCase()),
        onFilterDropdownOpenChange: (visible: boolean) => { filterDropdownAnimation(visible) },
        ellipsis: true,
        align: "center" as const,
        sorter: (a: DataItem, b: DataItem) => a.cname.localeCompare(b.cname),
        defaultSortOrder: "ascend" as const,
        hidden: false
    },
    {
        title: "Business Unit",
        dataIndex: "bu",
        key: "bu",
        customFilterDropdown: true,
        onFilter: (value: string | number | boolean, record: any) => 
            record.bu.toString().toLowerCase().includes(value.toString().toLowerCase()),
        onFilterDropdownOpenChange: (visible: boolean) => { filterDropdownAnimation(visible) },
        ellipsis: true,
        align: "center" as const,
        sorter: (a: DataItem, b: DataItem) => a.bu.localeCompare(b.bu),
        defaultSortOrder: "ascend" as const,
        hidden: false
    },
    {
        title: "Team Number",
        dataIndex: "tnr",
        key: "tnr",
        customFilterDropdown: true,
        onFilter: (value: string | number | boolean, record: any) => 
            record.tnr.toString().toLowerCase().includes(value.toString().toLowerCase()),
        onFilterDropdownOpenChange: (visible: boolean) => { filterDropdownAnimation(visible) },
        ellipsis: true,
        align: "center" as const,
        sorter: (a: DataItem, b: DataItem) => a.tnr- b.tnr,
        defaultSortOrder: "ascend" as const,
        hidden: false
    }
];

/*  Search implementation  */

//saves state of searched text and in which column
const state = reactive({
    searchText: "",
    searchedColumn: "",
});

const searchInput = ref();

/**
 * Saves the searched string and the target column in state, when search is confirmed.
 * @param {string[]} selectedKeys Has the searched text in the first position.
 * @param {((param?: FilterConfirmProps) => void)} confirm Confirms the search.
 * @param {string} dataIndex Has the target column.
 */
function handleSearch(selectedKeys: string[], confirm: ((param?: FilterConfirmProps) => void), dataIndex: string) {
    confirm();
    state.searchText = selectedKeys[0];
    state.searchedColumn = dataIndex;
}

/**
 * Resets the filtered search in target column.
 * @param {((param?: FilterResetProps) => void)} clearFilters Clears the filter, when confirmed.
 */
function handleReset(clearFilters: ((param?: FilterResetProps) => void)) {   
    clearFilters({ confirm: true });
    state.searchText = "";
}

/*  Column drop implementation  */

/**
 * Changes the visible columns based on the width of the left pane.
 * @param {number} pwidth Has the width of the left pane.
 */
function changeColumns(pwidth: number) {
    const breakpoint = getBreakpoint(pwidth);
    switch (breakpoint) {
        case "xs":
            hideColumn("cname");
            hideColumn("bu");
            hideColumn("tnr");
            break;
        case "sm":
            showColumn("cname");
            hideColumn("bu");
            hideColumn("tnr");
            break;
        case "md":
            showColumn("cname");
            showColumn("bu");
            hideColumn("tnr");
            break;
        case "lg":
            showColumn("cname");
            showColumn("bu");
            showColumn("tnr");
            break;
        default:
            hideColumn("cname");
            hideColumn("bu");
            hideColumn("tnr");
            break;
    }
}

/**
 * Hides given column.
 * @param {number} index Has the dataIndex of the column to hide.
 */
function hideColumn(index: string) {
    columns.forEach(column => {
        if (column.dataIndex == index) {
            column.hidden = true;
        }
    });
}

/**
 * Shows given column.
 * @param {number} index Has the dataIndex of the column to show.
 */
function showColumn(index: string) {
    columns.forEach(column => {
        if (column.dataIndex == index) {
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
    const windowSize = useWindowSize().width.value
    const breakpoint: number[] = [0.25 * windowSize, 0.42 * windowSize, 0.5 * windowSize]

    if (pwidth > breakpoint[2]) {
        return "lg";
    } else if (pwidth > breakpoint[1]) {
        return "md";
    } else if (pwidth > breakpoint[0]) {
        return "sm";
    } else {
        return "xs";
    }
         
}

</script>

<style>
    .highlight{
        background-color: rgb(255, 192, 105);
        padding: 0px;
    }
</style>