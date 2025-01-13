<script lang="ts" setup>
  import type { PropType } from 'vue';
  import type { ProjectEditStore } from '@/store/ProjectEditStore/ProjectEditStore';

  type Status = '' | 'error' | 'warning' | undefined;

  const { columnName, inputValue, inputStatus, editStore } = defineProps({
    columnName: {
      type: String,
      required: true,
    },
    inputValue: {
      type: [Number, String],
      required: true,
    },
    inputStatus: {
      type: Object as PropType<Status>,
      required: true,
    },
    editStore: {
      type: Object as PropType<ProjectEditStore>,
    },
  });

  const emit = defineEmits(['updated', 'error', 'success']);
</script>

<template>
  <a-input
    v-model:value="inputValue"
    class="inputField"
    :status="inputStatus"
    @input="emit('updated')"
    @change="
      () => {
        if (!inputValue) {
          emit('error');
          editStore?.addEmptyProjectInformationField(columnName);
        } else {
          emit('success');
          editStore?.removeEmptyProjectInformationField(columnName);
        }
      }
    "
  />
</template>
