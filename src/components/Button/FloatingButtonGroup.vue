<script setup lang="ts">
  import FloatingButton from './FloatingButton.vue';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';

  const props = defineProps({
    buttons: {
      type: Array<FloatButtonModel>,
      required: true,
    },
  });
</script>

<template>
  <a-float-button-group>
    <FloatingButton
      v-for="button in props.buttons"
      :key="button.name"
      :button="button"
    >
    </FloatingButton>
  </a-float-button-group>
</template>
