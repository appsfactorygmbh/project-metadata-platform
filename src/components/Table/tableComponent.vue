<script lang="ts" setup>
    import { SmileOutlined, SearchOutlined } from "@ant-design/icons-vue";
    import { reactive, ref, watch } from "vue";
    import { useWindowSize } from "@vueuse/core";
    import type { ColumnsType } from "ant-design-vue/es/table";

    /*const props = defineProps({
        projectName: {
            type: String,
            required: true,
        },
        clientName: {
            type: String,
            required: true,
        },
        bussinesUnit: {
            type: String,
            required: true,
        },
        teamNumber: {
            type: String,
            required: true,
        }
    })*/

    const props = defineProps({
        paneWidth: Number,
        paneHeight: Number
    });

    watch(
        () => props.paneWidth,
        () => {
            changeColumns(props.paneWidth)
        }
    )
    
</script>

<template>
    <a-table
        :columns="[...columns].filter(item => !item.hidden)" 
        :data-source="dataSource" 
        :pagination="false"
        :scroll="{ y: 0.904*useWindowSize().height.value}"
        bordered
    >
        <template #headerCell="{ column }">
            <template v-if="column.key === 'name'">
            <span>
                <smile-outlined />
                Name
            </span>
            </template>
        </template>

        <template
            #customFilterDropdown="{ setSelectedKeys, selectedKeys, confirm, clearFilters, column }"
        >
            <div style="padding: 8px">
                <a-input
                    ref="searchInput"
                    :placeholder="`Search ${column.dataIndex}`"
                    :value="selectedKeys[0]"
                    style="width: 188px; margin-bottom: 8px; display: block"
                    @change="e => setSelectedKeys(e.target.value ? [e.target.value] : [])"
                    @pressEnter="handleSearch(selectedKeys, confirm, column.dataIndex)"
                />
                <a-button
                    type="primary"
                    size="small"
                    style="width: 90px; margin-right: 8px"
                    @click="handleSearch(selectedKeys, confirm, column.dataIndex)"
                >
                    <template #icon><SearchOutlined /></template>
                    Search
                </a-button>
                <a-button size="small" style="width: 90px" @click="handleReset(clearFilters)">
                    Reset
                </a-button>
            </div>
        </template>

        <template #customFilterIcon="{ filtered }">
            <search-outlined :style="{ color: filtered ? '#108ee9' : undefined }" />
        </template>

        <template #bodyCell="{ text, column }">
            <span v-if="state.searchText && state.searchedColumn === column.dataIndex">
                <template
                    v-for="(fragment, i) in text
                        .toString()
                        .split(new RegExp(`(?<=${state.searchText})|(?=${state.searchText})`, 'i'))"
                    >
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

interface DataItem {
    key: number;
    pname: string;
    cname: string;
    bu: string;
    tnr: string;
}

const dataSource: DataItem[] = [];

for (let i = 0; i < 50; i++) {
    dataSource.push({
    key: i,
    pname: "Softwareprojekt",
    cname: "Appsfactory",
    bu: "Frontend",
    tnr: `${i}`
})    
}

const state = reactive({
    searchText: "",
    searchedColumn: "",
});

const searchInput = ref();

const filterDropdownAnimation = () => {
    if (visible) {
        setTimeout(() => {
            searchInput.value.focus();
        }, 100);
    }
}

const getColumnInfo = (title: string, index: string, key: string): ColumnsType => {
    return {
        title: title,
        dataIndex: index,
        key: key,
        align: "center",
        customFilterDropdown: true,
        onFilter: (value, record) => record[index].toString().toLowerCase().includes(value.toLowerCase()),
        onFilterDropdownOpenChange: visible => { filterDropdownAnimation },
        ellipsis: true,
        hidden: false
    }
}

//sets up columns
const columns: ColumnsType = [
  getColumnInfo("Project Name", "pname", "pname"),
  getColumnInfo("Client Name", "cname", "cname"),
  getColumnInfo("Business Unit", "bu", "bu"),
  getColumnInfo("Team Number", "tnr", "tnr")
];

function handleSearch(selectedKeys, confirm, dataIndex) {
    confirm();
    state.searchText = selectedKeys[0];
    state.searchedColumn = dataIndex;
}

function handleReset(clearFilters) {
    clearFilters({ confirm: true });
    state.searchText = "";
}


/*function addTableEntry(data: any): void {
        dataSource.push({
            key: dataSource.length + 1,
            pname: data.projectName,
            cname: data.clientName,
            bu: data.bussinesUnit,
            tnr: data.teamNumber
        })
}*/

function changeColumns(pwidth: number) {
    const size = getSize(pwidth);
    switch (size) {
        case "xs":
            showColumn(0);
            hideColumn(1);
            hideColumn(2);
            hideColumn(3);
            break;
        case "sm":
            showColumn(0);
            showColumn(1);
            hideColumn(2);
            hideColumn(3);
            break;
        case "md":
            showColumn(0);
            showColumn(1);
            showColumn(2);
            hideColumn(3);
            break;
        case "lg":
            showColumn(0);
            showColumn(1);
            showColumn(2);
            showColumn(3);
            break;
        default:
            break;
    }
    console.log(columns)
    dataSource.push({
    key: 0,
    pname: "Softwareprojekt",
    cname: "Appsfactory",
    bu: "Frontend",
    tnr: "0"
})  
    console.log(dataSource)
}

function hideColumn(id: number) {
    columns[id].hidden = true
}

function showColumn(id: number) {
    columns[id].hidden = false
}

function getSize(pwidth: number): string {
    const windowSize = useWindowSize().width.value
    const breakpoint: number[] = [0.25 * windowSize, 0.5 * windowSize, 0.75 * windowSize]
    
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