import { ref } from 'vue';
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

  const startEditing = () => {
    console.log('start editing');
    isEditing.value = true;
    router.push({
      path: router.currentRoute.value.path,
      query: { ...router.currentRoute.value.query, isEditing: 'true' },
    });
  };

  const stopEditing = () => {
    console.log('stop editing');
    isEditing.value = false;
    router.push({
      path: router.currentRoute.value.path,
      query: { ...router.currentRoute.value.query, isEditing: 'false' },
    });
  };

  return { isEditing, startEditing, stopEditing };
};
