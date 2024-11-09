<script lang="ts" setup>
  import { UsersApi } from '@/api/generated';
  import { userService } from '@/services';
  import { useUserStore } from '@/store';
  import { userStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { useAuth } from 'vue-auth3';

  const userStore = useUserStore();
  provide<typeof userStore>(userStoreSymbol, userStore);

  const auth = useAuth();
  userService.initApi(auth.token(), UsersApi);
  watch(
    () => auth.token(),
    () => {
      console.log('token change', auth);
      userService.initApi(auth.token(), UsersApi);
    },
  );
</script>
<template>
  <slot></slot>
</template>
