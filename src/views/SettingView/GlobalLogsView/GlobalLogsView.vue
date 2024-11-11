<script setup lang="ts">
  import type { LogEntryModel } from '@/models/Log/LogEntryModel';
  import { logsStoreSymbol } from '@/store/injectionSymbols';
  import { debounce } from 'lodash';
  import { inject, onMounted } from 'vue';
  const searchValue = ref('');
  const dummyData: LogEntryModel[] = [
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30+00:00').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp: new Date('2024-11-11T18:10:30Z').toLocaleString(),
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

<template>
  <div class="container">
    <a-input
      v-model:value="searchValue"
      placeholder="Search global logs"
      class="input"
      @change="updateSearchParam"
    />
    <a-card class="card">
      <div class="timeline">
        <LogTimeline
          :log-entries="
            dummyData.filter((item) => {
              return item.logMessage.includes(searchValue);
            })
          "
        >
          <template #label>
            <h1>This is the header</h1>
          </template></LogTimeline
        >
      </div>
    </a-card>
  </div>
</template>

<style lang="css" scoped>
  .input {
    margin: 10px;
    width: 30em;
  }
  .card {
    overflow-y: auto;
    height: 100%;
    margin: 0 10px 0 10px;
  }
</style>
