<script lang="ts" setup>
  const props = defineProps({
    placeholder: {
      type: String,
      required: true,
    },
    default: {
      type: String,
      required: true,
    },
    precision: {
      type: Number,
      default: undefined,
    },
  });
</script>

<template>
  <a-input-number
    :controls="false"
    :placeholder="props.placeholder"
    :default-value="props.default"
    :precision="props.precision"
  />
</template>
