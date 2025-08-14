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
      requiredValue: {
        type: Boolean,
        required: true,
      },
    });

  const emit = defineEmits(['updated', 'error', 'success']);
  const inputField = ref(inputValue);

  const handleChange = () => {
    if (!inputField.value && requiredValue) {
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
    v-model:value="inputField"
    class="textArea"
    :status="inputStatus"
    @input="$emit('updated', inputField)"
    @change="handleChange"
    :auto-size="{ minRows: 3, maxRows: 10 }" :maxlength=500 :show-count="true"
  />
</template>
