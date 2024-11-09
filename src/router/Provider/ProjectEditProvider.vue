<script setup lang="ts">
  import { useProjectEditStore } from '@/store';
  import { projectEditStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { useAuth } from 'vue-auth3';
  import { projectsService } from '@/services';
  import { ProjectsApi } from '@/api/generated';

  const projectEditStore = useProjectEditStore();
  provide<typeof projectEditStore>(projectEditStoreSymbol, projectEditStore);

  const auth = useAuth();
  projectsService.initApi(auth.token(), ProjectsApi);
  watch(
    () => auth.token(),
    () => {
      console.log('token change', auth);
      projectsService.initApi(auth.token(), ProjectsApi);
    },
  );
</script>

<template>
  <slot></slot>
</template>
