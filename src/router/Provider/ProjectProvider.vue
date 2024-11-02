<script lang="ts" setup>
  import { projectsService } from '@/services';
  import { useProjectStore } from '@/store';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { useAuth } from 'vue-auth3';

  const projectsStore = useProjectStore();
  provide<typeof projectsStore>(projectsStoreSymbol, projectsStore);

  const auth = useAuth();
  if (auth?.token()) {
    projectsService.setAuth(auth.token());
    watch(
      () => auth.token(),
      () => {
        console.log('token change', auth);
        projectsService.setAuth(auth.token());
      },
    );
  }
</script>
<template>
  <slot></slot>
</template>
