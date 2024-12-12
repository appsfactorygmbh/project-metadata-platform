<template>
  <div style="display: flex; align-items: center; width: 100%">
    <a-input-search
      placeholder="Type what you're looking for:"
      enter-button
      :value="searchStore?.getSearchQuery"
      :default-value="defaultSearchQuery"
      style="flex-grow: 1"
      @input="onInput"
    />
  </div>
</template>

<script lang="ts" setup>
  import { inject, onBeforeMount } from 'vue';
  import { type SearchStore } from '@/store/SearchStore';
  import type { Ref } from 'vue';
  import { useQuery } from '@/utils/hooks';
  import { useSessionStorage } from '@vueuse/core';

  const props = defineProps({
    searchStoreSymbol: {
      type: Symbol,
      required: true,
    },
  });

  const searchStorage = useSessionStorage('searchStorage', { searchQuery: '' });

  const { routerSearchQuery, setSearchQuery } = useQuery(['searchQuery']);
  const searchStore = inject<SearchStore>(props.searchStoreSymbol);

  const defaultSearchQuery: Ref<string> = ref('');

  // Input Listener
  const onInput = (e: Event) => {
    const target = e.target as HTMLInputElement;
    searchStore?.setSearchQuery(target.value);
    searchStorage.value.searchQuery = target.value;
  };

  watch(
    () => searchStore?.getSearchQuery,
    (newSearch) => {
      setTimeout(() => {
        if (newSearch != '') {
          setSearchQuery(newSearch, 'searchQuery');
        } else {
          setSearchQuery(undefined, 'searchQuery');
        }
      }, 0);
    },
  );

  onBeforeMount(() => {
    const routerQuery = routerSearchQuery.value[0];

    const searchQuery: string | undefined =
      routerQuery !== 'undefined' ? routerQuery : '';

    defaultSearchQuery.value = searchQuery || '';
    searchStore?.setSearchQuery(searchQuery || '');
  });
</script>
