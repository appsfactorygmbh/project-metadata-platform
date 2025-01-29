import { useProjectStore } from '@/store';
import { type Router, useRouter } from 'vue-router';

export const useProjectRouting = (router: Router = useRouter()) => {
  const routerProjectId = ref<number | undefined>(
    Number(router.currentRoute.value.query.projectId),
  );

  const routerProjectSlug = ref<string | undefined>(
    String(router.currentRoute.value.params.projectSlug),
  );

  const setProjectId = (id: number | undefined) => {
    if (id === undefined) {
      const { query, path } = router.currentRoute.value;
      const { projectId, ...remainingQuery } = query;

      router.replace({
        path,
        query: remainingQuery,
      });
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
