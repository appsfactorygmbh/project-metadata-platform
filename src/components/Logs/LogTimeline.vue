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
    <a-timeline mode="left" class="timetime">
      <a-timeline-item
        v-for="(data, index) in props.logEntries"
        :key="index"
        position="left"
        class="timelineItem"
      >
        <template #label>{{ data.timeStamp }}</template>
        <p class="logMessage">{{ data.logMessage }}</p>
      </a-timeline-item>
    </a-timeline>
  </div>
</template>

<style lang="scss" scoped>
  .logMessage {
    display: inline-block;
    white-space: normal;
    margin: 0;
  }

  .timetime {
    height: 89vh;
    display: flex;
    flex-direction: column;
  }
  .container {
    display: flex;
    align-items: flex-start;
    width: 100%;
  }
</style>
