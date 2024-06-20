<script setup lang="ts">
  import defaultTheme from '@/assets/themes/default-theme.json';
</script>
<template>
  <a-config-provider :theme="defaultTheme">
    <slot></slot>
  </a-config-provider>
</template>
