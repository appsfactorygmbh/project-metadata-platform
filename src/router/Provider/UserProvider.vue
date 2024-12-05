<script lang="ts" setup>
  import { userService } from '@/services';
  import { useUserStore } from '@/store';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { useAuth } from 'vue-auth3';

  const userStore = useUserStore();
  provide<typeof userStore>(userStoreSymbol, userStore);

  const auth = useAuth();
  userService.setAuth(auth?.token());
  watch(
    () => auth?.token(),
    () => {
      userService.setAuth(auth.token());
    },
  );
</script>
<template>
  <slot></slot>
</template>
