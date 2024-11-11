import { useRouter } from 'vue-router';

export const useEditing = () => {
  const router = useRouter();

  const isEditing = ref<boolean>(
    router.currentRoute.value.query.isEditing === 'true',
  );

  // Update isEditing when the URL query changes
  watch(
    () => router.currentRoute.value.query.isEditing,
    (newQueryIsEditing) => {
      isEditing.value = newQueryIsEditing === 'true';
    },
  );

  const startEditing = async () => {
    console.log('start editing');
    const currentQueries = router.currentRoute.value.query;
    await router.push({
      path: router.currentRoute.value.path,
      query: { ...currentQueries, isEditing: 'true' },
    });
  };

  const stopEditing = async () => {
    console.log('stop editing');
    const currentQueries = router.currentRoute.value.query;
    await router.push({
      path: router.currentRoute.value.path,
      query: { ...currentQueries, isEditing: 'false' },
    });
  };

  return { isEditing, startEditing, stopEditing };
};
