<script setup lang="ts">
  import { CloseOutlined, SaveOutlined } from '@ant-design/icons-vue';
  const emit = defineEmits(['cancel', 'save']);
</script>

<template>
  <a-float-button-group class="menu">
    <a-float-button class="button" ghost @click="() => emit('cancel')">
      <template #icon><CloseOutlined class="icon" /> </template>
    </a-float-button>
    <a-float-button class="button" ghost @click="() => emit('save')">
      <template #icon><SaveOutlined class="icon" /> </template>
    </a-float-button>
  </a-float-button-group>
</template>

<style scoped></style>
