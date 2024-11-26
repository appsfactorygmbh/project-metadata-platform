<script setup lang="ts">
  import { logsStoreSymbol } from '@/store/injectionSymbols';
  import { debounce } from 'lodash';
  import { inject, onMounted } from 'vue';
  const searchValue = ref('');

  const logsStore = inject(logsStoreSymbol)!;

  const updateSearchParam = debounce(async () => {
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
    <a-card
      class="card"
      :body-style="{
        height: '83vh',
      }"
    >
      <div class="timeline">
        <LogTimeline :log-entries="logsStore.getGlobalLogs" />
      </div>
    </a-card>
  </div>
</template>

<style lang="css" scoped>
  .input {
    margin: 15px;
    width: 30em;
  }
  .card {
    overflow-y: auto;
    height: 100%;
    margin: 0 15px 0 15px;
  }
</style>
