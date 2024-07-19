import { ref, computed, inject, watch } from 'vue';
import { useRouter } from 'vue-router';
import {
  pluginStoreSymbol,
  projectsStoreSymbol,
} from '@/store/injectionSymbols';

export const useEditing = () => {
  const router = useRouter();
  const pluginStore = inject(pluginStoreSymbol);
  const projectStore = inject(projectsStoreSymbol);

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
    console.log('start editing ', router.currentRoute.value.path);
    const currentQueries = router.currentRoute.value.query;
    router.push({
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
    const projectID = computed(() => projectStore?.getProject?.id);
    if (projectID.value) {
      pluginStore?.fetchPlugins(projectID.value);
    }
  };

  return { isEditing, startEditing, stopEditing };
};
