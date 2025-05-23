import { type Ref, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';

export interface TeamRoutingReturnType {
  routerTeamId: Ref<string>;
  setTeamId: (id: string | null) => void;
}

export const useTeamRouting = (): TeamRoutingReturnType => {
  const router = useRouter();
  const route = useRoute();

  const routerTeamId = ref<string>(String(route.query.teamId ?? ''));

  watch(
    () => route.query.teamId,
    (newQueryUserId) => {
      const newIdString = String(newQueryUserId ?? '');
      if (routerTeamId.value !== newIdString) {
        routerTeamId.value = newIdString;
      }
    },
  );

  const setTeamId = (id: string | null) => {
    const currentPath = route.path;
    const currentQuery = { ...route.query };

    if (id && id !== '0' && id !== 'undefined') {
      currentQuery.teamId = id;
    } else {
      delete currentQuery.teamId;
    }
    router.push({ path: currentPath, query: currentQuery }).catch((err) => {
      if (err.name !== 'NavigationDuplicated') {
      }
    });
  };

  return { routerTeamId, setTeamId };
};
