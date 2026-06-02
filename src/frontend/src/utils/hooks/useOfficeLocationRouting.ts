import { type Ref, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';

export interface OfficeLocationRoutingReturnType {
  routerOfficeLocationId: Ref<string>;
  setOfficeLocationId: (id: string | null) => void;
}

export const useOfficeLocationRouting = (): OfficeLocationRoutingReturnType => {
  const router = useRouter();
  const route = useRoute();

  const routerOfficeLocationId = ref<string>(String(route.query.officeLocationId ?? ''));

  watch(
    () => route.query.officeLocationId,
    (newQueryUserId) => {
      const newIdString = String(newQueryUserId ?? '');
      if (routerOfficeLocationId.value !== newIdString) {
        routerOfficeLocationId.value = newIdString;
      }
    },
  );

  const setOfficeLocationId = (id: string | null) => {
    const currentPath = route.path;
    const currentQuery = { ...route.query };

    if (id && id !== '0' && id !== 'undefined') {
      currentQuery.officeLocationId = id;
    } else {
      delete currentQuery.officeLocationId;
    }
    router.push({ path: currentPath, query: currentQuery }).catch((err) => {
      if (err.name !== 'NavigationDuplicated') {
        console.log(`unknown error: ${err}`);
      }
    });
  };

  return { routerOfficeLocationId, setOfficeLocationId };
};
