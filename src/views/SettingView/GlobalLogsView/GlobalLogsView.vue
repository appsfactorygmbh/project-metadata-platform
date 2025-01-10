<script setup lang="ts">
  import type { LogEntryModel } from '@/models/Log/LogEntryModel';
  import { logsStoreSymbol } from '@/store/injectionSymbols';
  import { debounce } from 'lodash';
  import { inject, onBeforeMount } from 'vue';
  const searchValue = ref('');
  const filteredLog = ref<LogEntryModel[]>([]);
  const logsStore = inject(logsStoreSymbol)!;

  const loadingGlobalLogs = computed(() => logsStore.getIsLoadingGlobalLogs);

  const updateSearchParam = debounce(async () => {
    await logsStore?.fetch(searchValue.value);
  }, 500);
  onBeforeMount(async () => {
    await logsStore?.fetch();
  });

  watch(
    () => filteredLog.value,
    (value) => {
      console.log(value);
    },
  );
</script>

<template>
  <div class="container">
    <a-space class="time" direction="horizontal" :size="12">
      <a-input
        v-model:value="searchValue"
        placeholder="Search global logs"
        class="input"
        @change="updateSearchParam"
      />
      <LogTimeFilter
        :log-entries="logsStore.getGlobalLogs"
        v-model:logs="filteredLog"
      />
    </a-space>
    <a-card class="cardContainer">
      <a-spin v-if="loadingGlobalLogs" class="loadingIcon" />
      <LogTimeline
        v-else-if="logsStore.getGlobalLogs && filteredLog.length > 0"
        :log-entries="filteredLog"
      />
      <a-flex v-else justify="center" align="center" style="height: 80vh">
        <a-empty description="No results found" />
      </a-flex>
    </a-card>
  </div>
</template>

<style lang="css" scoped>
  .container {
    height: 100vh;
    display: flex;
    flex-direction: column;
  }

  .input {
    width: 30em;
  }

  .time {
    margin-bottom: 15px;
  }

  .cardContainer {
    flex: 1;
    background-color: white;
    padding: 1em;
    overflow: auto;
    margin-bottom: 60px;
  }
  .loadingIcon {
    display: flex;
    justify-content: center;
  }
</style>
