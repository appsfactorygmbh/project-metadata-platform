import { useRouter } from 'vue-router';

export const useProjectRouting = () => {
  const router = useRouter();

  const routerProjectId = ref<number>(
    Number(router.currentRoute.value.query.projectId) || 0,
  );

  const setProjectId = (id: number) => {
    routerProjectId.value = id;
    router.push({
      path: router.currentRoute.value.path,
      query: {
        ...router.currentRoute.value.query,
        projectId: routerProjectId.value,
      },
    });
  };

  return { routerProjectId, setProjectId };
};
