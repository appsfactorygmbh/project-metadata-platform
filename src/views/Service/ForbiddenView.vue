<script lang="ts" setup></script>

<template>
  <a-flex justify="center" align="center" style="height: 100vh">
    <a-empty description="You are not allowed to access this.">
      <a-button type="primary" @click="$router.push({ path: '/' })">
        Go To Home
      </a-button>
    </a-empty>
  </a-flex>
</template>
