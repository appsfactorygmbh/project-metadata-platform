<script setup lang="ts">
  import type { LogEntryModel } from '@/models/Log';

  const props = defineProps({
    logEntries: {
      type: Array<LogEntryModel>,
      required: true,
    },
  });

  const getTimeStamp = (entry: LogEntryModel, index: number) => {
    if (index === props.logEntries.length - 1) return entry.timestamp;

    const nextEntry = props.logEntries[index + 1];
    const entryActor = entry.logMessage.split(' ').shift();
    const nextActor = nextEntry.logMessage.split(' ').shift();
    if (entry.timestamp == nextEntry.timestamp && entryActor == nextActor)
      return undefined;

    return entry.timestamp;
  };
</script>

<template>
  <LogItem
    v-for="(entry, index) in props.logEntries"
    :key="index"
    :log-message="entry.logMessage"
    :time-stamp="getTimeStamp(entry, index)"
    :is-last="index === props.logEntries.length - 1"
  />
</template>
