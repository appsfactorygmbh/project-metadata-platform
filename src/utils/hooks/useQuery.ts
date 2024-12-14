import { useRouter } from 'vue-router';
import type { ComputedRef } from 'vue';
import { ref } from 'vue';
import _ from 'lodash';
const isSearchQueryEmpty = ref(false);

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

  const setSearchQuery = async (
    searchQuery: string | undefined,
    queryName: string,
  ) => {
    const index = queryNames.findIndex((name) => name === queryName);
    routerSearchQuery.value[index] = searchQuery;
    isSearchQueryEmpty.value = _.isEmpty(searchQuery);

    await router.push({
      path: router.currentRoute.value.path,
      query: {
        ...router.currentRoute.value.query,
        [queryName]: routerSearchQuery.value[index],
      },
    });
  };

  return { queryNames, routerSearchQuery, setSearchQuery, isSearchQueryEmpty };
};
