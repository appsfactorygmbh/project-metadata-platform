<script setup lang="ts">
  import { useTheme } from '@/utils/hooks';

  const { themeConfig } = useTheme();
</script>
<template>
  <a-config-provider :theme="themeConfig">
    <a-app>
      <slot />
    </a-app>
  </a-config-provider>
</template>
