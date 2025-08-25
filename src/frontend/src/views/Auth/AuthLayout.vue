<script setup lang="ts">
  import { useThemeToken } from '@/utils/hooks';
  const token = useThemeToken();
</script>
<template>
  <section class="container">
    <a-card class="loginCard">
      <div>
        <slot />
      </div>
    </a-card>
  </section>
</template>

<style scoped>
  .container {
    display: flex;
    align-items: center;
    justify-content: center;
    height: 100vh;
    width: 100%;
    background-color: v-bind('token.colorFill');
  }
  .loginCard {
    padding: 2em;
    background-color: v-bind('token.colorBgElevated');
    box-shadow: v-bind('token.boxShadowSecondary');
  }
</style>
