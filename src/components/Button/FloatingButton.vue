<script setup lang="ts">
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';
  import type { PropType } from 'vue';

  const props = defineProps({
    button: {
      type: Object as PropType<FloatButtonModel>,
      required: true,
    },
  });
</script>

<template>
  <a-float-button
    v-if="
      props.button.status == 'activated' || props.button.status === undefined
    "
    :class="[props.button.size, props.button.specialType]"
    :type="props.button.type"
    :shape="props.button.shape"
    :tooltip="props.button.tooltip"
    :badge="props.button.badge"
    @click="props.button.onClick"
  >
    <template #icon>
      <component
        :is="props.button.icon"
        :style="props.button.color"
      ></component>
    </template>
  </a-float-button>
  <a-float-button
    v-else-if="props.button.status == 'disabled'"
    class="disabled-button"
    :type="props.button.type"
    :shape="props.button.shape"
    tooltip="This button is disabled"
  >
    <template #icon>
      <component :is="props.button.icon"></component>
    </template>
  </a-float-button>
</template>

<style>
  .disabled-button {
    cursor: not-allowed;
    filter: opacity(50%);
  }

  .ant-float-btn-body {
    width: 100% !important;
    height: 100% !important;
  }

  .ant-float-btn.large {
    transform: scale(1.2);
    + .ant-float-btn {
      margin-top: 1.5em;
    }
  }
  .ant-float-btn.middle {
    transform: scale(1);
  }
  .ant-float-btn.small {
    transform: scale(0.8);
  }

  .danger .ant-float-btn-body {
    background-color: color-mix(in srgb, #6d6e6f, #ff002e 60%) !important;
  }
  .danger .ant-float-btn-body:hover {
    background-color: color-mix(in srgb, #6d6e6f, #ff002e 80%) !important;
  }

  .success .ant-float-btn-body {
    background-color: color-mix(in srgb, #6d6e6f, #27d157 60%) !important;
  }
  .success .ant-float-btn-body:hover {
    background-color: color-mix(in srgb, #6d6e6f, #27d157 80%) !important;
  }
</style>
