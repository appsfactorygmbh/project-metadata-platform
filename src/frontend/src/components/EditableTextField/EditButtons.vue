<script setup lang="ts">
  import { useEditing } from '@/utils/hooks';
  import {
    CheckOutlined,
    CloseOutlined,
    EditOutlined,
  } from '@ant-design/icons-vue';
  import type { FormStore } from '../Form';

  const props = defineProps({
    isEditingKey: {
      type: String,
      required: true,
    },
    safeDisabled: {
      type: Boolean,
      required: true,
    },
    isLoading: {
      type: Boolean,
      required: true,
    },
    formStore: {
      type: Object as PropType<FormStore>,
      required: true,
    },
  });
  const emit = defineEmits(['savedChanges']);

  const { isEditing, startEditing, stopEditing } = useEditing(
    props.isEditingKey,
  );

  const safeEdit = async () => {
    await props.formStore.submit();

    emit('savedChanges');
    props.formStore.resetFields();

    console.log('save changes emitted (EDIT BUTTONS)');
    stopEditing();
  };

  const cancleEdit = () => {
    props.formStore.resetFields();
    stopEditing();
  };
</script>

<template>
  <a-button v-if="!isEditing" class="button" @click="startEditing">
    <EditOutlined />
  </a-button>
  <div v-else class="buttonGroup">
    <a-button
      class="check button"
      :disabled="props.safeDisabled"
      @click="safeEdit"
    >
      <CheckOutlined class="icon" />
    </a-button>
    <a-button class="abort button" @click="cancleEdit">
      <CloseOutlined class="icon" />
    </a-button>
  </div>
</template>

<style lang="css" scoped>
  .abort {
    background-color: color-mix(in srgb, #6d6e6f, #ff002e 60%) !important;
  }
  .abort:hover {
    background-color: color-mix(in srgb, #6d6e6f, #ff002e 80%) !important;
  }

  .check {
    background-color: color-mix(in srgb, #6d6e6f, #27d157 60%) !important;
  }
  .check:hover {
    background-color: color-mix(in srgb, #6d6e6f, #27d157 80%) !important;
  }

  .button {
    display: flex;
    justify-content: center;
    align-items: center;
    margin-left: auto;
  }

  .icon {
    color: white;
  }

  .buttonGroup {
    display: flex;
    flex-direction: row;
    margin-left: auto;
    margin-top: 0;
    margin-bottom: 0;
    gap: 10px;
  }
</style>
