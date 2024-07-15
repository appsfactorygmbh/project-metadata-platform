import _ from 'lodash';
import { useRouter } from 'vue-router';
import type { ComputedRef } from 'vue';

export const useQuery = (queryNames: string[]) => {
  const router = useRouter();

  const routerSearchQuery: ComputedRef<(string | undefined)[]> = computed(
    () => {
      return queryNames.map(
        (queryName) =>
          String(router.currentRoute.value.query[queryName]) || undefined,
      );
    },
  );

  const setSearchQuery = (
    searchQuery: string | undefined,
    queryName: string,
  ) => {
    const index = _.findIndex(queryNames, (name) => name === queryName);
    routerSearchQuery.value[index] = searchQuery;
    router.push({
      path: router.currentRoute.value.path,
      query: {
        ...router.currentRoute.value.query,
        [queryName]: routerSearchQuery.value[index],
      },
    });
  };

  return { queryNames, routerSearchQuery, setSearchQuery };
};
