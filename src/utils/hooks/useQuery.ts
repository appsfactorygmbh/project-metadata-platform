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

  const isSearchQuery: ComputedRef<boolean> = computed(() => {
    return queryNames
      .map((queryName) => router.currentRoute.value.query[queryName])
      .some((value) => value !== undefined && value !== 'undefined');
  });

  const setSearchQuery = async (
    searchQuery: string | undefined,
    queryName: string,
  ) => {
    const index = queryNames.findIndex((name) => name === queryName);
    routerSearchQuery.value[index] = searchQuery;
    const debugString = `set search query to : ${router.currentRoute.value.query}, ${queryName}: ${routerSearchQuery.value[index]}`;
    console.log(debugString);

    await router.push({
      path: router.currentRoute.value.path,
      query: {
        ...router.currentRoute.value.query,
        [queryName]: routerSearchQuery.value[index],
      },
    });
  };

  return { queryNames, routerSearchQuery, setSearchQuery, isSearchQuery };
};
