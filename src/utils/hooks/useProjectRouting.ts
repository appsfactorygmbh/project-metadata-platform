import { type Router, useRouter } from 'vue-router';

export const useProjectRouting = (router: Router = useRouter()) => {
  const routerProjectId = ref<number>(
    Number(router.currentRoute.value.query.projectId),
  );

  const routerProjectSlug = ref<string>(
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
    }
  };

  return { router, routerProjectId, routerProjectSlug, setProjectId };
};
