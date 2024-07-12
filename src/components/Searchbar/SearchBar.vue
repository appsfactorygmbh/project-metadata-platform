<template>
  <!-- Ant-Design input component -->
  <a-input-search
    placeholder="Type what you're looking for:"
    enter-button
    :default-value="defaultSearchQuery"
    @input="onInput"
  />
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
