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
  <a-timeline class="timeline">
    <a-timeline-item
      v-for="(data, index) in props.logEntries"
      :key="index"
      color="blue"
    >
      <div class="item">
        <p class="label">{{ data.timeStamp }}</p>
        <p>{{ data.logMessage }}</p>
      </div>
    </a-timeline-item>
  </a-timeline>
</template>

<style lang="scss" scoped>
  .timeline {
    box-sizing: border-box;
    height: max-content;
    .item {
      margin: 0;
      padding: 0;
      display: flex;
      flex-direction: row;

      p {
        margin: 0;
        width: max-content;
      }
    }

    .label {
      margin-right: 5em;
      min-width: 5em;
    }
  }
</style>
