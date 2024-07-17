<script lang="ts" setup>
  import { AuthLayout } from '.';
  import { computed } from 'vue';
  import { useToken } from 'ant-design-vue/es/theme/internal';
  import useBreakpoint from 'ant-design-vue/es/_util/hooks/useBreakpoint';

  const token = useToken()[1];
  const screens = useBreakpoint();

  const styles = computed(() => ({
    header: {
      marginBottom: `${token.value.marginXL}px`,
    },
    text: {
      color: token.value.colorTextSecondary,
    },
    title: {
      fontSize: screens.value.md
        ? token.value.fontSizeHeading2
        : token.value.fontSizeHeading3,
    },
  }));
</script>

<template>
  <AuthLayout>
    <div :style="styles.header">
      <a-typography-title :style="styles.title">Register</a-typography-title>
      <a-typography-text :style="styles.text">
        This feature is currently disabled.
      </a-typography-text>
      <RouterLink to="/login">Back to login</RouterLink>
    </div>
  </AuthLayout>
</template>
