import { useRouter } from 'vue-router';

export const useEditing = (queryKey: string = 'isEditing') => {
  const router = useRouter();

  const isEditing = ref<boolean>(
    router.currentRoute.value.query[queryKey] === 'true',
  );

  // Update isEditing when the URL query changes
  watch(
    () => router.currentRoute.value.query[queryKey],
    (newQueryIsEditing) => {
      isEditing.value = newQueryIsEditing === 'true';
    },
  );

  const startEditing = async () => {
    console.log('start editing');
    const currentQueries = router.currentRoute.value.query;
    await router.push({
      path: router.currentRoute.value.path,
      query: { ...currentQueries, [queryKey]: 'true' },
    });
  };

  const stopEditing = async () => {
    const currentQueries = router.currentRoute.value.query;
    await router.push({
      path: router.currentRoute.value.path,
      query: { ...currentQueries, [queryKey]: 'false' },
    });
  };

  return { isEditing, startEditing, stopEditing };
};
