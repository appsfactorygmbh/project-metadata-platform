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
  });
</script>

<template>
  <a-input :placeholder="props.placeholder" :default-value="props.default" />
</template>
