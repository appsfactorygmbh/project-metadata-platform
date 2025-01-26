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
      type: String as PropType<Status>,
      required: true,
    },
    editStore: {
      type: Object as PropType<ProjectEditStore>,
      required: true,
    },
  });

  const emit = defineEmits(['updated', 'error', 'success']);
  const inputField = ref(inputValue);

  const handleChange = () => {
    if (!inputField.value) {
      emit('error');
      editStore?.addEmptyProjectInformationField(columnName);
    } else {
      emit('success');
      editStore?.removeEmptyProjectInformationField(columnName);
    }
  };
</script>

<template>
  <a-input
    v-model:value="inputField"
    class="inputField"
    :status="inputStatus"
    @input="$emit('updated', inputField)"
    @change="handleChange"
  />
</template>
