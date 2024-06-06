<script lang="ts">
    const props = defineProps({
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
    })

</script>

<template>    
    <a-table 
        :columns="columns" 
        :data-source="dataSource" 
        :pagination="false"
        >
        <template #headerCell="{ column }">
            <template v-if="column.key === 'name'">
            <span>
                <smile-outlined />
                Name
            </span>
            </template>
        </template>

        <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'name'">
            <a>
                {{ record.name }}
            </a>
            </template>
        </template>
    </a-table>
</template>

<script lang="ts" setup>
import { SmileOutlined } from "@ant-design/icons-vue";
import type { ColumnsType } from "ant-design-vue/es/table";

const columns: ColumnsType = [
  {
    title: "Project Name",
    dataIndex: "pname",
    key: "pname",
  },
  {
    title: "Client Name",
    dataIndex: "cname",
    key: "cname",
    responsive: ["md"],
  },
  {
    title: "Business Unit",
    dataIndex: "bu",
    key: "bu",
    responsive: ["md"],
  },
  {
    title: "Team Nr.",
    key: "tnr",
    dataIndex: "tnr",
    responsive: ["lg"],
  }
];

interface DataItem {
    key: number;
    pname: string;
    cname: string;
    bu: string;
    tnr: string;
}

const dataSource: DataItem[] = [];

dataSource.push({
    key: 0,
    pname: "Softwareprojekt",
    cname: "Appsfactory",
    bu: "Frontend",
    tnr: "1"
})

function addTableEntry(data: any): void {
        dataSource.push({
            key: dataSource.length + 1,
            pname: data.projectName,
            cname: data.clientName,
            bu: data.bussinesUnit,
            tnr: data.teamNumber
        })
    }
</script>