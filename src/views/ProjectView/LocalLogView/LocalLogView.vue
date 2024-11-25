<script lang="ts" setup>
  import { inject } from 'vue';
  import {
    localLogStoreSymbol,
    projectRoutingSymbol,
  } from '@/store/injectionSymbols';

  const { routerProjectId } = inject(projectRoutingSymbol)!;
  const localLogStore = inject(localLogStoreSymbol)!;
  //const { getLocalLog, getIsLoadingLocalLog } = storeToRefs(localLogStore);
  //const log = computed(() => getLocalLog?.value);

  //const isLoading = computed(
  //  () => getIsLoadingLocalLog.value,
  //);

  watch(
    () => routerProjectId.value,
    async () => {
      await localLogStore?.fetchLocalLog(routerProjectId.value);
    },
  );
</script>

<template>
  <a-timeline class="LocalLog" style="margin: 2em 5em">
    <a-timeline-item>Create a services site 2015-09-01</a-timeline-item>
    <a-timeline-item>Solve initial network problems 2015-09-01</a-timeline-item>
    <a-timeline-item>Technical testing 2015-09-01</a-timeline-item>
    <a-timeline-item>Network problems being solved 2015-09-01</a-timeline-item>
  </a-timeline>
</template>
<style>
  .LocalLog {
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    background-color: white;
    border-radius: 15px;
    padding: 1em;
  }
</style>
