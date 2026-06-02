import { type Ref, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';

export interface CompanyRoutingReturnType {
  routerCompanyId: Ref<string>;
  setCompanyId: (id: string | null) => void;
}

export const useCompanyRouting = (): CompanyRoutingReturnType => {
  const router = useRouter();
  const route = useRoute();

  const routerCompanyId = ref<string>(String(route.query.companyId ?? ''));

  watch(
    () => route.query.companyId,
    (newQueryUserId) => {
      const newIdString = String(newQueryUserId ?? '');
      if (routerCompanyId.value !== newIdString) {
        routerCompanyId.value = newIdString;
      }
    },
  );

  const setCompanyId = (id: string | null) => {
    const currentPath = route.path;
    const currentQuery = { ...route.query };

    if (id && id !== '0' && id !== 'undefined') {
      currentQuery.companyId = id;
    } else {
      delete currentQuery.companyId;
    }
    router.push({ path: currentPath, query: currentQuery }).catch((err) => {
      if (err.name !== 'NavigationDuplicated') {
        console.log(`unknown error: ${err}`);
      }
    });
  };

  return { routerCompanyId, setCompanyId };
};
