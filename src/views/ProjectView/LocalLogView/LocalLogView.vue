<script lang="ts" setup>
  import { inject } from 'vue';
  import { storeToRefs } from 'pinia';
  import { localLogStoreSymbol } from '@/store/injectionSymbols';

  const localLogStore = inject(localLogStoreSymbol)!;
  const { getLocalLog, getIsLoadingLocalLog } = storeToRefs(localLogStore);
  const logsData = computed(() => getLocalLog?.value);
  const isLoading = computed(() => getIsLoadingLocalLog.value);
</script>

<template>
  <div v-if="logsData.length > 0" class="localLog">
    <a-card v-if="!isLoading" class="cardContainer">
      <LogTimeline :log-entries="logsData" class="timeline" />
    </a-card>

    <a-skeleton v-else active :paragraph="false" style="padding: 2em" />
  </div>
</template>
<style>
  .LocalLog {
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    background-color: white;
    border-radius: 15px;
    margin: 2em 5em;
  }

  .cardContainer {
    flex: 1;
    background-color: white;
    padding: 1em;
    overflow: auto;
    margin-bottom: 60px;
    margin: 1em;
    border: none;
  }

  .timeline {
    height: 100%;
  }
</style>
