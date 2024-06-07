<script lang="ts" setup>
    import { SmileOutlined, SearchOutlined } from "@ant-design/icons-vue";
    import { reactive, ref, watch } from "vue";
    import { useWindowSize } from "@vueuse/core";

    /*const props = defineProps({
        pname: {
            type: String,
            required: true,
        },
        cname: {
            type: String,
            required: true,
        },
        bu: {
            type: String,
            required: true,
        },
        tnr: {
            type: String,
            required: true,
        }
    })*/

    //Get the width of the pane from App.vue
    const props = defineProps({
        paneWidth: {
            type: Number,
            required: true
        },
        paneHeight: Number
    });

    //update the Pane-width when the pane is resized
    watch(
        () => props.paneWidth,
        () => {
            changeColumns(props.paneWidth);
        }
    )
    
</script>

<template>
    <!-- 
        Ant Design table with: 
        columns: filtered if hidden or not
        scroll: sets height to 0.9 of window size 
    -->
    <a-table
        :columns="[...columns].filter(item => !item.hidden)" 
        :data-source="[...dataSource]" 
        :pagination="false"
        :scroll="{ y: 900 }"
        bordered
    >
        <!-- header of the table -->
        <template #headerCell="{ column }">
            <template v-if="column.key === 'name'">
            <span>
                <smile-outlined />
                Name
            </span>
            </template>
        </template>

        <!--  -->
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

/*  Data implementation  */

interface DataItem {
    key: number;
    pname: string;
    cname: string;
    bu: string;
    tnr: string;
}

const dataSource: DataItem[] = [];

function addTableEntry(data: any): void {
        dataSource.push({
            key: dataSource.length + 1,
            pname: data.pname,
            cname: data.cname,
            bu: data.bu,
            tnr: data.tnr
        })
        console.log(dataSource)
}

//Test data
const testData = []

for (let i = 0; i < 50; i++) {
    testData.push({
        key: i,
        pname: "Softwareprojekt",
        cname: "Appsfactory",
        bu: "Frontend",
        tnr: `${i}`
    })
}

testData.forEach((data) => {
    addTableEntry(data);
});

/*  Column implementation  */

const fillColumn = (title: string, index: string, key: string) => {
    return {
        title: title,
        dataIndex: index,
        key: key,
        customFilterDropdown: true,
        onFilter: (value: string | number | boolean  , record: any) => record[index].toString().toLowerCase().includes(value.toString().toLowerCase()),
        onFilterDropdownOpenChange: (visible: any) => { filterDropdownAnimation(visible) },
        ellipsis: true,
        align: "center" as const,
        hidden: false
    }
};

const filterDropdownAnimation = (visible: any) => {
    if (visible) {
        setTimeout(() => {
            searchInput.value.focus();
        }, 100);
    }
};

const columns = [
    fillColumn("Project Name", "pname", "pname"),
    fillColumn("Client Name", "cname", "cname"),
    fillColumn("Business Unit", "bu", "bu"),
    fillColumn("Team Number", "tnr", "tnr")
];

/*  Search implementation  */

const state = reactive({
    searchText: "",
    searchedColumn: "",
});

const searchInput = ref();

function handleSearch(selectedKeys: any, confirm: any, dataIndex: any) {
    confirm();
    state.searchText = selectedKeys[0];
    state.searchedColumn = dataIndex;
}

function handleReset(clearFilters: any) {
    clearFilters({ confirm: true });
    state.searchText = "";
}

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
            showColumn(0);
            hideColumn(1);
            hideColumn(2);
            hideColumn(3);
            break;
    }
}

function hideColumn(id: number) {
    columns[id].hidden = true
}

function showColumn(id: number) {
    columns[id].hidden = false
}

function getSize(pwidth: number): string {
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