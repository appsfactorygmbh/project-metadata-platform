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
defineProps({
  type: {
    type: String,
    default: 'text', //primary | ghost | dashed | link | text | default
  },
  shape: {
    type: String,
    default: 'circle',
  },
  size: {
    type: String,
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
