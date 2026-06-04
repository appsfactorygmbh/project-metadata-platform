import { type Ref, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';

export interface BusinessUnitRoutingReturnType {
  routerBusinessUnitId: Ref<string>;
  setBusinessUnitId: (id: string | null) => void;
}

export const useBusinessUnitRouting = (): BusinessUnitRoutingReturnType => {
  const router = useRouter();
  const route = useRoute();

  const routerBusinessUnitId = ref<string>(
    String(route.query.businessUnitId ?? ''),
  );

  watch(
    () => route.query.businessUnitId,
    (newQueryUserId) => {
      const newIdString = String(newQueryUserId ?? '');
      if (routerBusinessUnitId.value !== newIdString) {
        routerBusinessUnitId.value = newIdString;
      }
    },
  );

  const setBusinessUnitId = (id: string | null) => {
    const currentPath = route.path;
    const currentQuery = { ...route.query };

    if (id && id !== '0' && id !== 'undefined') {
      currentQuery.businessUnitId = id;
    } else {
      delete currentQuery.businessUnitId;
    }
    router.push({ path: currentPath, query: currentQuery }).catch((err) => {
      if (err.name !== 'NavigationDuplicated') {
        console.log(`unknown error: ${err}`);
      }
    });
  };

  return { routerBusinessUnitId, setBusinessUnitId };
};
