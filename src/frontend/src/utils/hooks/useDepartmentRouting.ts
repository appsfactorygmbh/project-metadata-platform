import { type Ref, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';

export interface DepartmentRoutingReturnType {
  routerDepartmentId: Ref<string>;
  setDepartmentId: (id: string | null) => void;
}

export const useDepartmentRouting = (): DepartmentRoutingReturnType => {
  const router = useRouter();
  const route = useRoute();

  const routerDepartmentId = ref<string>(String(route.query.departmentId ?? ''));

  watch(
    () => route.query.departmentId,
    (newQueryUserId) => {
      const newIdString = String(newQueryUserId ?? '');
      if (routerDepartmentId.value !== newIdString) {
        routerDepartmentId.value = newIdString;
      }
    },
  );

  const setDepartmentId = (id: string | null) => {
    const currentPath = route.path;
    const currentQuery = { ...route.query };

    if (id && id !== '0' && id !== 'undefined') {
      currentQuery.departmentId = id;
    } else {
      delete currentQuery.departmentId;
    }
    router.push({ path: currentPath, query: currentQuery }).catch((err) => {
      if (err.name !== 'NavigationDuplicated') {
        console.log(`unknown error: ${err}`);
      }
    });
  };

  return { routerDepartmentId, setDepartmentId };
};
