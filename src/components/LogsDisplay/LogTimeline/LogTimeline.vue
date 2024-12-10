<script setup lang="ts">
  import type { LogEntryModel } from '@/models/Log';

  const props = defineProps({
    logEntries: {
      type: Array<LogEntryModel>,
      required: true,
    },
  });
  console.log(props.logEntries);
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
