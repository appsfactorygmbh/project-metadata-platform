<template>
  <a-tooltip
    v-if="tooltip"
    :title="tooltip"
    :placement="tooltipPosition"
  >
    <span>
      <a-button
        :type="type"
        :shape="shape"
        :size="size"
        class="button"
        @click="$emit('click')"
      >
        <template #icon>
          <slot name="icon" />
        </template>
      </a-button>
    </span>
  </a-tooltip>
  <a-button
    v-else
    :type="type"
    :shape="shape"
    :size="size"
    class="button"
    @click="$emit('click')"
  >
    <template #icon>
      <slot name="icon" />
    </template>
  </a-button>
</template>

<script lang="ts" setup>
  import type { ButtonShape, ButtonType } from 'ant-design-vue/es/button';
  import type { SizeType } from 'ant-design-vue/es/config-provider';
  import type { TooltipPlacement } from 'ant-design-vue/es/tooltip';

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
    tooltip: {
      type: String,
      default: '',
    },
    tooltipPosition: {
      type: String as () => TooltipPlacement,
      default: 'left',
    },
  });

  defineEmits(['click']);
</script>

<style scoped>
  .button {
    margin-top: 10px;
    display: inline-flex;
    justify-content: center;
    align-items: center;
  }
</style>
