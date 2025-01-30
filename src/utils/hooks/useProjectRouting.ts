import { useProjectStore } from '@/store';
import { type Router, useRouter } from 'vue-router';

export const useProjectRouting = (router: Router = useRouter()) => {
  const projectIdRoute = router.currentRoute.value.query.projectId;
  const projectSlugRoute = router.currentRoute.value.params.projectSlug;

  const routerProjectId = ref<number | undefined>(Number(projectIdRoute));

  const routerProjectSlug = ref<string | undefined>(
    projectSlugRoute ? String(projectSlugRoute) : undefined,
  );

  const setProjectId = (id: number | undefined) => {
    if (id === undefined) {
      const { query, path } = router.currentRoute.value;
      const { projectId, ...remainingQuery } = query;

      router.replace({
        path,
        query: remainingQuery,
      });

      routerProjectSlug.value = undefined;
    } else {
      routerProjectId.value = id;
      router.push({
        path: router.currentRoute.value.path,
        query: {
          ...router.currentRoute.value.query,
          projectId: routerProjectId.value,
        },
      });

      updateProjectSlug(id);
    }
  };

  const updateProjectSlug = async (id: number) => {
    const projectStore = useProjectStore();
    const project = await projectStore.findProjectById(id, {
      fullObjectNeeded: false,
    });
    const projectSlug = project?.slug;
    routerProjectSlug.value = projectSlug!;
  };

  return {
    router,
    routerProjectId,
    routerProjectSlug,
    setProjectId,
  };
};
