<script setup lang="ts">
  import type { LocalLogModel } from '@/models/LocalLogs';

  const props = defineProps({
    logEntries: {
      type: Array<LocalLogModel>,
      required: true,
    },
  });
</script>

<template>
  <LogItem
    v-for="(entry, index) in props.logEntries"
    :key="index"
    class="timeLine"
    :log-message="entry.logMessage"
    :time-stamp="entry.timestamp"
    :is-last="index === props.logEntries.length - 1"
  />
</template>
