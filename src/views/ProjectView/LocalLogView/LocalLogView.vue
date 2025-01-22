<script lang="ts" setup>
  import { inject } from 'vue';
  import { storeToRefs } from 'pinia';
  import { localLogStoreSymbol } from '@/store/injectionSymbols';
  import { useThemeToken } from '@/utils/hooks';

  const token = useThemeToken();

  const localLogStore = inject(localLogStoreSymbol)!;
  const { getLocalLog, getIsLoadingLocalLog } = storeToRefs(localLogStore);
  const logsData = computed(() => getLocalLog?.value);
  const isLoading = computed(() => getIsLoadingLocalLog.value);
</script>

<template>
  <div v-if="logsData.length > 0" class="localLog">
    <a-card v-if="!isLoading" class="cardContainer">
      <LogTimeline :log-entries="logsData.slice().reverse()" />
    </a-card>
    <a-skeleton v-else active :paragraph="false" style="padding: 2em" />
  </div>
</template>
<style>
  .LocalLog {
    margin: 2em 5em;
  }

  .cardContainer {
    flex: 1;
    justify-self: center;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    background-color: v-bind('token.colorBgElevated');
    border-radius: 15px;
    overflow: auto;
    margin-bottom: 60px;
    padding-top: 0.6em;
    border: none;
  }
</style>
