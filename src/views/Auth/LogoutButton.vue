<script lang="ts" setup>
  import { useAuth } from 'vue-auth3';

  const auth = useAuth();

  const logout = () => {
    auth.logout();
    window.location.reload();
  };
</script>

<template>
  <a-button
    type="primary"
    @click="logout"
  >
    Logout
  </a-button>
</template>
