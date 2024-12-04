<script setup lang="ts">
  import { CloseOutlined, SaveOutlined } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '../Button';
  const emit = defineEmits(['cancel', 'save']);

  const buttons: FloatButtonModel[] = [
    {
      name: 'CancelButton',
      onClick: () => {
        emit('cancel');
      },
      icon: CloseOutlined,
      status: 'activated',
      type: 'primary',
      size: 'large',
      specialType: 'danger',
      tooltip: 'Click to cancel editing',
    },
    {
      name: 'SaveButton',
      onClick: () => {
        emit('save');
      },
      icon: SaveOutlined,
      status: 'activated',
      type: 'primary',
      size: 'large',
      specialType: 'success',
      tooltip: 'Click to save changes',
    },
  ];
</script>

<template>
  <FloatingButtonGroup :buttons="buttons" />
</template>

<style scoped></style>
