import { useRouter } from 'vue-router';

export const useUserRouting = () => {
  const router = useRouter();

  const routerUserId = ref<string>(
    String(router.currentRoute.value.query.userId) || '',
  );

  const setUserId = (id: string) => {
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
