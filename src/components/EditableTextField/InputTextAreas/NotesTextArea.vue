<script lang="ts" setup>
  import type { PropType } from 'vue';
  import type { ProjectEditStore } from '@/store/ProjectEditStore/ProjectEditStore';

  type Status = '' | 'error' | 'warning' | undefined;

  const { columnName, inputValue, inputStatus, editStore, requiredValue } =
    defineProps({
      columnName: {
        type: String,
        required: true,
      },
      requiredValue: {
        type: Boolean,
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
  const textArea = ref(inputValue);

  const handleChange = () => {
    if (!textArea.value && requiredValue) {
      emit('error');
      editStore?.addEmptyProjectInformationField(columnName);
    } else {
      emit('success');
      editStore?.removeEmptyProjectInformationField(columnName);
    }
  };
</script>

<template>
  <a-textarea
    v-model:value="textArea"
    class="textArea"
    :status="inputStatus"
    :auto-size="{ minRows: 3, maxRows: 10 }"
    :maxlength="500"
    :show-count="true"
    @input="$emit('updated', textArea)"
    @change="handleChange"
  />
</template>
