<script setup lang="ts">
  import {
    CheckOutlined,
    CloseOutlined,
    EditOutlined,
  } from '@ant-design/icons-vue';
  const props = defineProps({
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
  <a-button v-if="!isEditing" class="button" @click="emit('startEditing')">
    <EditOutlined />
  </a-button>
  <div v-else class="buttonGroup">
    <a-button
      class="check button"
      :disabled="props.safeDisabled"
      @click="emit('safeEdits')"
    >
      <CheckOutlined class="icon" />
    </a-button>
    <a-button class="abort button" @click="emit('cancleEdit')">
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
