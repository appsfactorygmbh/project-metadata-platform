<template>
  <a-input-search
    v-model:value="value"
    placeholder="Type what you're looking for:"
    enter-button
    @input="onInput"
  />
  <a-table
    :columns="columns"
    :dataSource="filteredData"
    rowKey="id"
    :pagination="false"
  />
</template>

<script lang="ts" setup>
  import { ref, computed } from 'vue';
  const value = ref<string>('');

  // Test-Table from UserStory
  const columns = [
    { 
      title: 'Project Name', 
      dataIndex: 'projectName', 
      key: 'projectName' 
    },
    { 
      title: 'Client Name', 
      dataIndex: 'clientName', 
      key: 'clientName' 
    },
    { 
      title: 'Business Unit', 
      dataIndex: 'businessUnit', 
      key: 'businessUnit' 
    },
    { 
      title: 'Team Nr.', 
      dataIndex: 'teamNumber', 
      key: 'teamNumber' 
    },
  ];

  // Test-Data for Table 
  const searchData = ref([
    {
      id: 1,
      projectName: 'Metadata Platform',
      clientName: 'Heinrich Krämer',
      businessUnit: 'BU 1',
      teamNumber: '1',
    },
    {
      id: 2,
      projectName: 'Portfolio Website',
      clientName: 'Nicolas Grzywacz',
      businessUnit: 'BU 2',
      teamNumber: '2',
    },
    {
      id: 3,
      projectName: 'Messenger Alike',
      clientName: 'Philipp Müller',
      businessUnit: 'BU 3',
      teamNumber: '3',
    },
    {
      id: 3,
      projectName: 'Hatti Tatti',
      clientName: 'John Snow',
      businessUnit: 'BU 1',
      teamNumber: '1',
    },
    {
      id: 4,
      projectName: 'Footbal Manager',
      clientName: 'Cristiano Ronaldo',
      businessUnit: 'BU 2',
      teamNumber: '3',
    },
    {
      id: 5,
      projectName: 'Messenger Alike',
      clientName: 'Heinrich Krämer',
      businessUnit: 'BU 2',
      teamNumber: '3',
    },
  ]);

  // Filtering table data
  const filteredData = computed(() => {
    if (!value.value) {
      return searchData.value;
    }
    return searchData.value.filter(
      (item) =>
        item.projectName.toLowerCase().includes(value.value.toLowerCase()) ||
        item.clientName.toLowerCase().includes(value.value.toLowerCase()) ||
        item.businessUnit.toLowerCase().includes(value.value.toLowerCase()) ||
        item.teamNumber.toLowerCase().includes(value.value.toLowerCase()),
    );
  });

  // Input Listener
  const onInput = (e: Event) => {
    const target = e.target as HTMLInputElement;
    value.value = target.value;
  };
</script>

<style></style>
