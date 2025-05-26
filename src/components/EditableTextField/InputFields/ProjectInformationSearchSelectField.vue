<script lang="ts" setup>
  import type { PropType } from 'vue';
  import type { ProjectEditStore } from '@/store/ProjectEditStore/ProjectEditStore';
  import type { SelectHandler } from 'ant-design-vue/es/vc-select/Select';

  type Status = '' | 'error' | 'warning' | undefined;

  const props = defineProps({
    columnName: {
      type: String,
      required: true,
    },
    inputValue: {
      type: [String, Number],
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
    options: {
      type: Array as PropType<string[]>,
      required: true,
    },
    isEditing: {
      type: Boolean,
      required: true,
    },
    displayValue: {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      type: Function as PropType<(value: any) => any>,
      required: false,
      default: (value: unknown) => value,
    },
    getValue: {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      type: Function as PropType<(value: any) => any>,
      required: false,
      default: (value: unknown) => value,
    },
  });

  const emit = defineEmits(['updated', 'error', 'success']);
  const selectField = ref(props.displayValue(props.inputValue));

  const handleChange = () => {
    if (!selectField.value) {
      emit('error');
      props.editStore?.addEmptyProjectInformationField(props.columnName);
    } else {
      emit('success');
      props.editStore?.removeEmptyProjectInformationField(props.columnName);
    }
  };

  const handleSelect: SelectHandler = (value: string) => {
    const transformedValue = props.getValue(value);
    emit('updated', transformedValue);
  };
</script>

<template>
  <a-select
    v-if="isEditing"
    v-model:value="selectField"
    show-search
    placeholder="Select a team"
    style="width: 200px"
    @change="handleChange"
    @select="handleSelect"
  >
    <a-select-option v-for="option in options" :key="option" :value="option">
      {{ option }}
    </a-select-option>
  </a-select>
</template>

<style scoped>
  .selectField {
    width: 100%;
  }
</style>
