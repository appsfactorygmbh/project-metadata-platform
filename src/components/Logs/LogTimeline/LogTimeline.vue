<script setup lang="ts">
  import type { LogEntryModel } from '@/models/Log/LogEntryModel';

  const props = defineProps({
    logEntries: {
      type: Array<LogEntryModel>,
      required: true,
    },
  });
</script>

<template>
  <div class="container">
    <LogItem
      v-for="(entry, index) in props.logEntries"
      :key="index"
      class="timeLine"
      :log-message="entry.logMessage"
      :time-stamp="entry.timestamp"
      :is-last="index === props.logEntries.length - 1"
    />
  </div>
</template>
