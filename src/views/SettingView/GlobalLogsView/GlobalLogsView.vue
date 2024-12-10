<script setup lang="ts">
  import { logsStoreSymbol } from '@/store/injectionSymbols';
  import { debounce } from 'lodash';
  import { inject, onMounted } from 'vue';
  const searchValue = ref('');

  const logsStore = inject(logsStoreSymbol)!;

  const updateSearchParam = debounce(async () => {
    await logsStore?.fetch(searchValue.value);
  }, 500);
  onMounted(async () => {
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
      <LogTimeline
        v-if="logsStore.getGlobalLogs && logsStore.getGlobalLogs.length > 0"
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
</style>
