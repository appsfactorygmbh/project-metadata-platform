import { type Ref, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';

export interface ApiTokenRoutingReturnType {
  routerApiTokenId: Ref<string>;
  setApiTokenId: (id: string | null) => void;
}

export const useApiTokenRouting = (): ApiTokenRoutingReturnType => {
  const router = useRouter();
  const route = useRoute();

  const routerApiTokenId = ref<string>(String(route.query.apiTokenId ?? ''));

  watch(
    () => route.query.apiTokenId,
    (newQueryApiTokenId) => {
      const newIdString = String(newQueryApiTokenId ?? '');
      if (routerApiTokenId.value !== newIdString) {
        routerApiTokenId.value = newIdString;
      }
    },
  );

  const setApiTokenId = (id: string | null) => {
    const currentPath = route.path;
    const currentQuery = { ...route.query };

    if (id && id !== '0' && id !== 'undefined') {
      currentQuery.apiTokenId = id;
    } else {
      delete currentQuery.apiTokenId;
    }
    router.push({ path: currentPath, query: currentQuery }).catch((err) => {
      if (err.name !== 'NavigationDuplicated') {
        console.log(`unknown error: ${err}`);
      }
    });
  };

  return { routerApiTokenId: routerApiTokenId, setApiTokenId: setApiTokenId };
};
