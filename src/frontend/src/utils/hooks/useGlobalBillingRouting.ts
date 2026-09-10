import { type Ref, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';

export interface GlobalBillingRoutingReturnType {
  routerGlobalBillingId: Ref<string>;
  setGlobalBillingId: (id: string | null) => void;
}

export const useGlobalBillingRouting = (): GlobalBillingRoutingReturnType => {
  const router = useRouter();
  const route = useRoute();

  const routerGlobalBillingId = ref<string>(
    String(route.query.billingId ?? ''),
  );

  watch(
    () => route.query.billingId,
    (newQueryUserId) => {
      const newIdString = String(newQueryUserId ?? '');
      if (routerGlobalBillingId.value !== newIdString) {
        routerGlobalBillingId.value = newIdString;
      }
    },
  );

  const setGlobalBillingId = (id: string | null) => {
    const currentPath = route.path;
    const currentQuery = { ...route.query };

    if (id && id !== '0' && id !== 'undefined') {
      currentQuery.billingId = id;
    } else {
      delete currentQuery.billingId;
    }
    router.push({ path: currentPath, query: currentQuery }).catch((err) => {
      if (err.name !== 'NavigationDuplicated') {
        console.log(`unknown error: ${err}`);
      }
    });
  };

  return { routerGlobalBillingId, setGlobalBillingId };
};
