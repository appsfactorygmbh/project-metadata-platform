<script setup lang="ts">
  import { useProjectEditStore } from '@/store';
  import { projectEditStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { useAuth } from 'vue-auth3';
  import { projectsService } from '@/services';

  const projectEditStore = useProjectEditStore();
  provide<typeof projectEditStore>(projectEditStoreSymbol, projectEditStore);

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
