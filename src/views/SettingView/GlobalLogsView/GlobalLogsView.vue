<script setup lang="ts">
  import { logsStoreSymbol } from '@/store/injectionSymbols';
  import { debounce } from 'lodash';
  import { inject, onBeforeMount } from 'vue';
  const searchValue = ref('');

  const logsStore = inject(logsStoreSymbol)!;

  const loadingGlobalLogs = computed(() => logsStore.getIsLoadingGlobalLogs);

  const updateSearchParam = debounce(async () => {
    await logsStore?.fetch(searchValue.value);
  }, 500);
  onBeforeMount(async () => {
    console.log('mouting');
    await logsStore?.fetch();
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
    <a-card class="cardContainer">
      <a-spin v-if="loadingGlobalLogs" class="loadingIcon" />
      <LogTimeline
        v-else-if="
          logsStore.getGlobalLogs && logsStore.getGlobalLogs.length > 0
        "
        :log-entries="logsStore.getGlobalLogs"
        class="timeline"
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
    margin-bottom: 15px !important;
    width: 30em;
  }

  .cardContainer {
    flex: 1;
    background-color: white;
    padding: 1em;
    overflow: auto;
    margin-bottom: 60px;
  }

  .timeline {
    height: 100%;
  }
  .loadingIcon {
    display: flex;
    justify-content: center;
  }
</style>
