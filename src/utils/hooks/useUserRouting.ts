import { useRouter } from 'vue-router';

export const useUserRouting = () => {
  const router = useRouter();

  const routerUserId = ref<number>(
    Number(router.currentRoute.value.query.userId) || 0,
  );

  const setUserId = (id: number) => {
    routerUserId.value = id;
    router.push({
      path: router.currentRoute.value.path,
      query: {
        userId: routerUserId.value,
      },
    });
  };

  return { routerUserId, setUserId };
};
