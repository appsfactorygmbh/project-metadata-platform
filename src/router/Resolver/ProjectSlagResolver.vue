<script lang="ts" setup>
  import { watch, inject } from 'vue';
  import { useRoute, useRouter } from 'vue-router';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';

  const route = useRoute();
  const router = useRouter();

  const projectsStore = inject(projectsStoreSymbol);

  watch(
    () => route.query.projectId,
    async (newId, oldId) => {
      if (newId !== oldId && newId !== undefined) {
        const projectId = parseInt(String(newId));
        const projectSlag = await projectsStore?.getProjectSlagById(projectId);

        const newQuery = route.query; // _.omit(route.query, 'projectId');
        router.replace({ query: { ...newQuery }, params: { projectSlag } });
      }
    },
  );
</script>
<template>
  <RouterView />
</template>
