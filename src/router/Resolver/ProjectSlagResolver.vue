<script lang="ts" setup>
  import { watch, inject, onMounted } from 'vue';
  import { useRoute, useRouter, type LocationQueryValue } from 'vue-router';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';

  const route = useRoute();
  const router = useRouter();

  const projectsStore = inject(projectsStoreSymbol);

  const redirectToSlag = async (
    id: LocationQueryValue | LocationQueryValue[],
    replace: boolean = true,
  ) => {
    const projectId = parseInt(String(id));
    const projectSlag = await projectsStore?.getProjectSlagById(projectId);
    if (!projectSlag) {
      return router.replace({
        query: { ...route.query },
        name: 'SplitViewDefault',
        replace,
      });
    }

    const newQuery = route.query;
    router.replace({
      query: { ...newQuery },
      params: { projectSlag },
      name: 'SplitView',
      replace,
    });
  };

  onMounted(async () => {
    await projectsStore;

    if (route.query.projectId) {
      await redirectToSlag(String(route.query.projectId));
    }
  });

  watch(
    () => route.query.projectId,
    async (newId, oldId) => {
      console.log('newId:', newId);
      if (newId !== oldId && newId !== undefined) {
        await redirectToSlag(newId);
      }
    },
  );
</script>
<template>
  <RouterView />
</template>
