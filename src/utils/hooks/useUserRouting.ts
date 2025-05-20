import { ref, watch, type Ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';

export interface UserRoutingReturnType {
  routerUserId: Ref<string>;
  setUserId: (id: string | null) => void;
}

export const useUserRouting = (): UserRoutingReturnType => {
  const router = useRouter();
  const route = useRoute();

  const routerUserId = ref<string>(String(route.query.userId ?? ''));

  watch(
    () => route.query.userId,
    (newQueryUserId) => {
      const newIdString = String(newQueryUserId ?? '');
      if (routerUserId.value !== newIdString) {
        routerUserId.value = newIdString;
      }
    }
  );

  const setUserId = (id: string | null) => {
    const currentPath = route.path;
    const currentQuery = { ...route.query };

    if (id && id !== '0' && id !== 'undefined') {
      currentQuery.userId = id;
    } else {
      delete currentQuery.userId;
    }
    router.push({ path: currentPath, query: currentQuery })
      .catch(err => {
        if (err.name !== 'NavigationDuplicated') {
        }
      });
  };

  return { routerUserId, setUserId };
};
