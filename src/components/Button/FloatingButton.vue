<script setup lang="ts">
  import type { ButtonModel } from '@/models/ButtonModel';
  import type { PropType } from 'vue';

  const props = defineProps({
    button: {
      type: Object as PropType<ButtonModel>,
      required: true,
    },
  });
</script>

<template>
  <a-float-button
    v-if="!props.button.disabled"
    :tooltip="props.button.tooltip"
    :disabled="props.button.disabled"
    @click="props.button.onClick"
  >
    <template #icon>
      <component :is="props.button.icon"></component>
    </template>
  </a-float-button>
</template>
