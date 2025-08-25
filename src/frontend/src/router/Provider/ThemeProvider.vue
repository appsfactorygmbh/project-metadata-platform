<script setup lang="ts">
  import { useTheme } from '@/utils/hooks';

  const { themeConfig } = useTheme();
</script>
<template>
  <a-config-provider :theme="themeConfig">
    <slot />
  </a-config-provider>
</template>
