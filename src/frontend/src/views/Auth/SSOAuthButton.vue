<script lang="ts" setup>
  import { msalService } from '@/services/msalService';

  const login = async () => {
    msalService.login();
  };
</script>

<template>
  <a-button block type="primary" @click="login"> Login with Azure </a-button>
</template>
