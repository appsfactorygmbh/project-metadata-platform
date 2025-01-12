<script lang="ts" setup>
  import type { LogEntryModel } from '@/models/Log/LogEntryModel';
  import type { Dayjs } from 'dayjs';
  import { ref } from 'vue';
  import type { CSSProperties } from 'vue';
  import dayjs from 'dayjs';
  import isBetween from 'dayjs/plugin/isBetween';
  dayjs.extend(isBetween);

  const props = defineProps({
    logEntries: {
      type: Array<LogEntryModel>,
      required: true,
    },
  });

  const emit = defineEmits(['update:logs']);
  const logs = ref<LogEntryModel[]>(props.logEntries);
  const date = ref<[Dayjs, Dayjs]>();

  const getCurrentStyle = (current: Dayjs) => {
    const style: CSSProperties = {};

    if (current.date() === 1) {
      style.border = '1px solid #1890ff';
      style.borderRadius = '50%';
    }

    return style;
  };

  function handleChange(value: [Dayjs, Dayjs] | undefined) {
    if (value) {
      logs.value = props.logEntries.filter((entry) => {
        const logDate = dayjs(entry.timestamp).startOf('day');
        return logDate.isBetween(
          value?.[0].startOf('day'),
          value?.[1].endOf('day'),
          null,
          '[]',
        );
      });
      emit('update:logs', logs.value);
    } else {
      emit('update:logs', props.logEntries);
    }
  }

  watch(
    () => props.logEntries,
    () => {
      handleChange(date.value);
    },
  );
  watch(
    () => date.value,
    (value) => {
      handleChange(value);
    },
  );
</script>

<template>
  <a-range-picker v-model:value="date">
    <template #dateRender="{ current }">
      <div class="ant-picker-cell-inner" :style="getCurrentStyle(current)">
        {{ current.date() }}
      </div>
    </template>
  </a-range-picker>
</template>
