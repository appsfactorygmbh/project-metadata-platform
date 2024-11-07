<script setup lang="ts">
  import type { LogEntryModel } from '@/models/Log/LogEntryModel';
  const currentdate = new Date();
  const dummyData: LogEntryModel[] = [
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
    {
      logMessage: 'Das ist ein Logeintrag. Ein nutzer wurde gelöscht.',
      timeStamp:
        currentdate.getDate() +
        '-' +
        (currentdate.getMonth() + 1) +
        '-' +
        currentdate.getFullYear(),
    },
  ];
</script>

<template class="test">
  <a-card class="card">
    <a-input placeholder="Search global Logs" class="input" />
    <LogTimeline :log-entries="dummyData"></LogTimeline>
  </a-card>
</template>

<style lang="css" scoped>
  .card {
    height: 90vh;
    overflow-y: auto;
    box-shadow: rgba(100, 100, 111, 0.2) 0 7px 29px 0 !important;
    margin: 20px;
    padding: 15px;
  }
  .input {
    margin-bottom: 20px;
    width: 30em;
  }
  .test {
    display: none;
  }
</style>
