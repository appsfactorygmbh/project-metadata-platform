<script setup lang="ts">
  import {
    CheckOutlined,
    CloseOutlined,
    EditOutlined,
  } from '@ant-design/icons-vue';
  defineProps({
    isEditing: {
      type: Boolean,
      required: true,
      default: false,
    },
    safeDisabled: {
      type: Boolean,
      required: true,
      default: false,
    },
    isLoading: {
      type: Boolean,
      required: true,
      default: false,
    },
  });
  const emit = defineEmits(['startEditing', 'cancleEdit', 'safeEdits']);
</script>

<template>
  <a-button v-if="!isEditing" class="edit" @click="emit('startEditing')">
    <EditOutlined class="icon" />
  </a-button>
  <div v-else class="buttonGroup">
    <a-button class="edit check" @click="emit('safeEdits')">
      <CheckOutlined class="icon" />
    </a-button>
    <a-button class="edit abort" @click="emit('cancleEdit')">
      <CloseOutlined class="icon" />
    </a-button>
  </div>
</template>
