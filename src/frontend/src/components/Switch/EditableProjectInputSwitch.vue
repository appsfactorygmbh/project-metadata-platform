<script lang="ts" setup>
  import { ref, watch, computed, type PropType } from 'vue';
  import type { ProjectEditStore } from '@/store/ProjectEditStore/ProjectEditStore';

  const props = defineProps({
    columnName: {
      type: String,
      required: true,
    },
    inputValue: {
      type: Boolean,
      required: true,
    },
    editStore: {
      type: Object as PropType<ProjectEditStore>,
      required: true,
    },
    isEditing: {
      type: Boolean,
      required: true,
    },
    requiredValue: {
      type: Boolean,
      required: true,
    },
  });

  const emit = defineEmits(['updated', 'error', 'success']);
  const inputField = ref(props.inputValue);

  watch(
    () => props.inputValue,
    (newVal) => {
      inputField.value = newVal;
    },
  );

  const displayLabel = computed(() => {
    return props.columnName === 'isEoC' ? 'EoC' : props.columnName;
  });

  const handleChange = (checked: boolean | string | number) => {
    const isChecked = checked === true;

    emit('updated', isChecked);
    emit('success');
    props.editStore?.removeEmptyProjectInformationField(props.columnName);
  };
</script>

<template>
  <a-card
    :body-style="{
      display: 'flex',
      padding: '5px',
      alignItems: 'center',
      height: 'fit-content',
      overflow: 'auto',
    }"
    class="info"
  >
    <label v-if="displayLabel != null" class="label">{{ displayLabel }}:</label>
    <a-switch
      v-model:checked="inputField"
      class="custom-color-switch"
      :class="{ editField: isEditing, viewField: !isEditing }"
      :disabled="!isEditing"
      @change="handleChange"
    />
  </a-card>
</template>

<style lang="css" scoped>
  .info {
    border: none;
    width: 100%;
    height: auto;
    max-width: 100%;
    font-size: 1.3em;
    font-weight: bold;
    display: flex;
    flex-flow: column wrap;
    justify-content: center;
    background-color: transparent;
  }

  .label {
    width: 5em;
    min-width: 5em;
    margin-right: 3em;
  }

  .formItem {
    margin: 0;
  }

  :deep(.custom-color-switch.ant-switch-checked),
  :deep(.custom-color-switch.ant-switch-checked:hover) {
    background-color: #27d157;
  }

  .editField {
    margin: 0 2em 0 1em;
  }

  .viewField {
    margin: 0 0 0 0.5em;
  }

  :deep(.custom-color-switch:not(.ant-switch-checked)),
  :deep(.custom-color-switch:not(.ant-switch-checked):hover) {
    background-color: #ff002e;
  }
</style>
