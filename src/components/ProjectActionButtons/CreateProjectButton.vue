<script setup lang="ts">
  import { defineEmits, ref } from 'vue';
  import { PlusOutlined } from '@ant-design/icons-vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';

  const emit = defineEmits(['update:open']);
  const openModal = ref(false);

  const showModal = () => {
    console.log('CreateProjectButton clicked'); // Debugging Log
    openModal.value = true;
    emit('update:open', true); // Send open status
  };

  const createButton: FloatButtonModel = {
    name: 'CreateProjectButton',
    onClick: showModal,
    icon: PlusOutlined,
    status: 'activated',
    tooltip: 'Click here to create a new project',
  };
</script>

<template>
  <FloatingButton :button="createButton" />
</template>
