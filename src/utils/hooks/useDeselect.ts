import { useRoute} from 'vue-router';

export const useDeselect = (queryKey: string = 'isDeselected') => {
  const route = useRoute();


  const isDeselected = ref<boolean>(route.fullPath === '/');

  // Update isDeselected when the URL query changes
  watch(
    () => route.fullPath,
    (fullRoute) => {
      isDeselected.value = fullRoute === '/';
    },
  );


  return { isDeselected };
};
