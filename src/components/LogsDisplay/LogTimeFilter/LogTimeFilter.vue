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

  const getCurrentStyle = (current: Dayjs, today: Dayjs) => {
    const style: CSSProperties = {};

    if (current.isSame(today,'date')) {
      style.border = '1px solid #3e8eef';
      style.borderRadius = '20%';    
    }

    return style;
  };

  function handleChange(inputDate: [Dayjs, Dayjs] | undefined) {
    if (inputDate) {
      logs.value = props.logEntries.filter((entry) => {
        const logDate = dayjs(entry.timestamp).startOf('day');
        return logDate.isBetween(
          inputDate?.[0].startOf('day'),
          inputDate?.[1].endOf('day'),
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
    <template #dateRender="{ current, today }">
      <div class="ant-picker-cell-inner" :style="getCurrentStyle(current, today)">
        {{ current.date() }}
      </div>
    </template>
  </a-range-picker>
</template>
