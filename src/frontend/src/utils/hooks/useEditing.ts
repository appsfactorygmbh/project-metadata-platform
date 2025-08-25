import { useRoute, useRouter } from 'vue-router';

export const useEditing = (queryKey: string = 'isEditing') => {
  const router = useRouter();
  const route = useRoute();

  const isEditing = ref<boolean>(route.query[queryKey] === 'true');

  // Update isEditing when the URL query changes
  watch(
    () => route.query[queryKey],
    (newQueryIsEditing) => {
      isEditing.value = newQueryIsEditing === 'true';
    },
  );

  const startEditing = async () => {
    const currentQueries = route.query;
    await router.push({
      path: route.path,
      query: { ...currentQueries, [queryKey]: 'true' },
    });
  };

  const stopEditing = async () => {
    const currentQueries = route.query;
    await router.push({
      path: route.path,
      query: { ...currentQueries, [queryKey]: 'false' },
    });
  };

  return { isEditing, startEditing, stopEditing };
};
