<script lang="ts" setup>
  import { ProjectsApi } from '@/api/generated';
  import { projectsService } from '@/services';
  import { useProjectStore } from '@/store';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { provide } from 'vue';
  import { useAuth } from 'vue-auth3';

  const projectsStore = useProjectStore();
  provide<typeof projectsStore>(projectsStoreSymbol, projectsStore);

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
