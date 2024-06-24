<script setup lang="ts">
  import type { FloatButtonModel } from '@/components/Button';
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
    v-if="!props.button.disabled && !props.button.deactivated"
    :type="props.button.type"
    :shape="props.button.shape"
    :tooltip="props.button.tooltip"
    @click="props.button.onClick"
  >
    <template #icon>
      <component :is="props.button.icon"></component>
    </template>
  </a-float-button>
  <a-float-button
    v-else-if="props.button.disabled && !props.button.deactivated"
    class="disabled-button"
    :type="props.button.type"
    :shape="props.button.shape"
  >
    <template #icon>
      <component :is="props.button.icon"></component>
    </template>
  </a-float-button>
</template>

<style>
  .disabled-button {
    cursor: not-allowed;
  }
</style>
