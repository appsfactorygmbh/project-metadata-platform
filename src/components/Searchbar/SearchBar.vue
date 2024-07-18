<template>
  <div style="display: flex; align-items: center; width: 100%">
    <a-input-search
      placeholder="Type what you're looking for:"
      enter-button
      :value="searchStore?.getSearchQuery"
      :default-value="defaultSearchQuery"
      style="flex-grow: 1; margin-right: 50px"
      @input="onInput"
    />
  </div>
</template>

<script lang="ts" setup>
  import { inject, onBeforeMount } from 'vue';
  import { type SearchStore } from '@/store/SearchStore';
  import type { Ref } from 'vue';
  import { useQuery } from '@/utils/hooks';

  const props = defineProps({
    searchStoreSymbol: {
      type: Symbol,
      required: true,
    },
  });

  const { routerSearchQuery, setSearchQuery } = useQuery(['searchQuery']);
  const searchStore = inject<SearchStore>(props.searchStoreSymbol);

  const defaultSearchQuery: Ref<string> = ref('');

  // Input Listener
  const onInput = (e: Event) => {
    const target = e.target as HTMLInputElement;
    if (target.value != '') {
      setSearchQuery(target.value, 'searchQuery');
    } else {
      setSearchQuery(undefined, 'searchQuery');
    }
    searchStore?.setSearchQuery(target.value);
  };

  onBeforeMount(() => {
    const routerQuery = routerSearchQuery.value[0];

    const searchQuery: string | undefined =
      routerQuery !== 'undefined' ? routerQuery : '';

    defaultSearchQuery.value = searchQuery || '';
    searchStore?.setSearchQuery(searchQuery || '');
  });
</script>
