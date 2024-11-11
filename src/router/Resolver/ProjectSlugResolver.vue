<script lang="ts" setup>
  import { onMounted, watch } from 'vue';
  import { type LocationQueryValue, useRoute, useRouter } from 'vue-router';
  import { useProjectStore } from '@/store';

  const route = useRoute();
  const router = useRouter();
  const projectStore = useProjectStore();

  const redirectToSlug = async (
    id: LocationQueryValue | LocationQueryValue[],
    replace: boolean = true,
  ) => {
    const projectId = parseInt(String(id));
    const projectSlug = await projectStore.getSlugById(projectId);
    if (!projectSlug) {
      return router.replace({
        query: { ...route.query },
        name: 'SplitViewDefault',
        replace,
      });
    }

    const newQuery = route.query;
    router.replace({
      query: { ...newQuery },
      params: { projectSlug },
      name: 'SplitView',
      replace,
    });
  };

  onMounted(async () => {
    if (route.query.projectId) {
      await redirectToSlug(String(route.query.projectId));
    }
  });

  watch(
    () => route.query.projectId,
    async (newId, oldId) => {
      console.log('newId:', newId);
      if (newId !== oldId && newId !== undefined) {
        await redirectToSlug(newId);
      }
    },
  );
</script>
<template>
  <RouterView />
</template>
