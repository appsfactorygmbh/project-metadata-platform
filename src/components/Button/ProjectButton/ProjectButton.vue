<template>
  <a-button
    :type="type"
    :shape="shape"
    :size="size"
    class="button"
    style="margin-left: 10px"
    @click="$emit('click')"
  >
    <template #icon>
      <slot name="icon"></slot>
    </template>
  </a-button>
</template>

<script lang="ts" setup>
import type { ButtonType, ButtonShape } from 'ant-design-vue/es/button';
import type { SizeType } from 'ant-design-vue/es/config-provider';

defineProps({
  type: {
    type: String as () => ButtonType,
    default: 'text',
  },
  shape: {
    type: String as () => ButtonShape,
    default: 'circle',
  },
  size: {
    type: String as () => SizeType,
    default: 'large',
  },
});
</script>

<style scoped>
.button {
  margin-top: 10px;
  display: inline-flex;
  justify-content: center;
  align-items: center;
}
</style>
