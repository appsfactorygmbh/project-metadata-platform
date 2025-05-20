import { type Router, useRouter } from 'vue-router';

export const useTeamRouting = (router: Router = useRouter()) => {
  const routerTeamId = ref<string>(
    String(router.currentRoute.value.query.teamId) || '',
  );

  const setTeamId = (id: string) => {
    routerTeamId.value = id;
    router.push({
      path: router.currentRoute.value.path,
      query: {
        teamId: routerTeamId.value,
      },
    });
  };

  return { routerTeamId, setTeamId };
};
