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
  <a-button v-if="!isEditing" class="edit button" @click="emit('startEditing')">
    <EditOutlined class="icon" />
  </a-button>
  <div v-else class="buttonGroup">
    <a-button class="check button" @click="emit('safeEdits')">
      <CheckOutlined class="icon" />
    </a-button>
    <a-button class="abort button" @click="emit('cancleEdit')">
      <CloseOutlined class="icon" />
    </a-button>
  </div>
</template>

<style lang="css" scoped>
  .check {
    background-color: #24c223;
  }

  .abort {
    background-color: #ff002e;
  }

  .button {
    width: 2em;
    height: 2em;
    display: flex;
    justify-content: center;
    align-items: center;
    border: none;
  }

  .edit {
    margin: 0.6em 0 0.6em;
    margin-left: auto;
    background-color: rgba(0, 0, 0, 0.88) !important;
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
