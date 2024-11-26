<script lang="ts" setup>
  import { inject } from 'vue';
  import { storeToRefs } from 'pinia';
  import { localLogStoreSymbol } from '@/store/injectionSymbols';

  const localLogStore = inject(localLogStoreSymbol)!;
  const { getLocalLog, getIsLoadingLocalLog } = storeToRefs(localLogStore);
  const logs = computed(() => getLocalLog?.value);
  const isLoading = computed(() => getIsLoadingLocalLog?.value);
</script>

<template>
  <div class="localLog">
    <a-timeline v-if="!isLoading" class="timeline" mode="left">
      <a-timeline-item v-for="log in logs" :key="log.timestamp" class="log">
        <template #label>{{ log.timestamp }}</template>
        {{ log.logMessage }}
      </a-timeline-item>
    </a-timeline>
    <a-skeleton
      v-else
      active
      :paragraph="false"
      style="padding: 0 2em 2em 2em"
    />
  </div>
</template>
<style>
  .LocalLog {
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    background-color: white;
    border-radius: 15px;
    padding-top: 2em;
    margin: 2em 5em;
  }

  .timeline {
    display: flex;
    align-items: start;
    flex-direction: column;
  }

  .ant-timeline-item-content {
    padding-right: 12em !important;
    width: auto !important;
  }
</style>
