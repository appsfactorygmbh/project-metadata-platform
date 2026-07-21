<script setup lang="ts">
  import {
    CloseOutlined,
    InboxOutlined,
    SaveOutlined,
  } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '../Button';

  const props = withDefaults(
    defineProps<{
      canEdit?: boolean;
    }>(),
    {
      canEdit: true,
    },
  );

  const emit = defineEmits(['cancel', 'save', 'archive']);

  const buttons = computed<FloatButtonModel[]>(() => [
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
      name: 'ArchiveButton',
      onClick: () => {
        emit('archive');
      },
      icon: InboxOutlined,
      status: props.canEdit ? 'activated' : 'disabled',
      type: 'primary',
      size: 'large',
      tooltip: 'Click to archive project',
    },
    {
      name: 'SaveButton',
      onClick: () => {
        emit('save');
      },
      icon: SaveOutlined,
      status: props.canEdit ? 'activated' : 'disabled',
      type: 'primary',
      size: 'large',
      specialType: 'success',
      tooltip: 'Click to save changes',
    },
  ]);
</script>

<template>
  <FloatingButtonGroup :buttons="buttons" />
</template>
