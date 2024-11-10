<script setup lang="ts">
  import type { LogEntryModel } from '@/models/Log/LogEntryModel';
  import { logsStoreSymbol } from '@/store/injectionSymbols';
  import { debounce } from 'lodash';
  import { inject, onMounted } from 'vue';
  const currentdate = new Date();
  const searchValue = ref('');
  const dummyData: LogEntryModel[] = [
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein Projekt wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
  ];

  const logsStore = inject(logsStoreSymbol);

  const updateSearchParam = debounce(async () => {
    console.log('update');
    await logsStore?.fetchGlobalLogs(searchValue.value);
  }, 500);
  onMounted(async () => {
    await logsStore?.fetchGlobalLogs();
  });
</script>

<template class="test">
  <a-card class="card">
    <a-input
      v-model:value="searchValue"
      placeholder="Search global Logs"
      class="input"
      @change="updateSearchParam"
    />
    <LogTimeline
      :log-entries="
        dummyData.filter((item) => {
          return item.logMessage.includes(searchValue);
        })
      "
    ></LogTimeline>
  </a-card>
</template>

<style lang="css" scoped>
  .card {
    height: 90vh;
    overflow-y: auto;
    box-shadow: rgba(100, 100, 111, 0.2) 0 7px 29px 0 !important;
    margin: 20px;
    padding: 15px;
  }
  .input {
    margin-bottom: 20px;
    width: 30em;
  }
  .test {
    display: none;
  }
</style>
