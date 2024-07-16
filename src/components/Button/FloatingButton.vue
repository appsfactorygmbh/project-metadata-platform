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
    :type="props.button.type"
    :shape="props.button.shape"
    :tooltip="props.button.tooltip"
    :badge="props.button.badge"
    @click="props.button.onClick"
  >
    <template #icon>
      <component :is="props.button.icon"></component>
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
</style>
