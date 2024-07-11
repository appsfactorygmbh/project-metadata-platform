<template>
  <!-- Ant-Design input component -->
  <a-input-search
    placeholder="Type what you're looking for:"
    enter-button
    @input="onInput"
  />
</template>

<script lang="ts" setup>
  import { inject, onMounted } from 'vue';
  import { type SearchStore } from '@/store/SearchStore';
  import { useRouter } from 'vue-router';

  const props = defineProps({
    searchStoreSymbol: {
      type: Symbol,
      required: true,
    },
  });

  const router = useRouter();
  const searchStore = inject<SearchStore>(props.searchStoreSymbol);

  // Input Listener
  const onInput = (e: Event) => {
    const target = e.target as HTMLInputElement;
    if (target.value != '') {
      pushSearchQuery(target.value);
    } else {
      pushSearchQuery(undefined);
    }
    searchStore?.setSearchQuery(target.value);
  };

  const pushSearchQuery = (searchQuery: string | undefined) => {
    router.push({
      path: router.currentRoute.value.path,
      query: { ...router.currentRoute.value.query, searchQuery: searchQuery },
    });
  };

  onMounted(() => {
    const searchQuery = router.currentRoute.value.query.searchQuery as string;
    searchStore?.setSearchQuery(searchQuery);
  });
</script>
