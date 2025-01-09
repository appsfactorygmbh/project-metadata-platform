<script lang="ts" setup>
import type { Dayjs } from 'dayjs';
import { ref } from 'vue';
import type { CSSProperties } from 'vue';

const emit = defineEmits(['update:date']);
const date = ref<[Dayjs, Dayjs]>();

const getCurrentStyle = (current: Dayjs) => {
  const style: CSSProperties = {};

  if (current.date() === 1) {
    style.border = '1px solid #1890ff';
    style.borderRadius = '50%';
  }

  return style;
};

watch(
  () => date.value,
  (value) => {
    emit('update:date', value);
  }
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