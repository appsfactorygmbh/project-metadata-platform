<script lang="ts" setup>
  </script>

<template>
  <div style="margin: 2em 5em">
    <a-timeline mode="alternate" class="localLog">
      <a-timeline-item class="logItem">
        <template #dot></template>
        MMustermann edited Team Number from 2 to 1
      </a-timeline-item>
      <a-timeline-item color="red" class="time"
        >2024-11-11T18:10:30+00:00</a-timeline-item
      >

      <a-timeline-item class="logItem">
        <template #dot></template>
        MMustermann deleted plugin Jira
      </a-timeline-item>
      <a-timeline-item color="red" class="time"
        >2024-11-11T18:10:30+00:00</a-timeline-item
      >

      <a-timeline-item class="logItem">
        <template #dot></template>
        MMustermann edited project name from "DB Apps" to "DB App"
      </a-timeline-item>
      <a-timeline-item color="red" class="time"
        >2024-11-11T18:10:30+00:00</a-timeline-item
      >
    </a-timeline>
  </div>
</template>
<style>
  .logItem {
    padding-top: 1em !important;
    padding-bottom: 0 !important;
    height: 0;
  }

  .LocalLog {
    margin: 1em;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    background-color: white;
    border-radius: 15px;
    padding: 1em;
  }
</style>
