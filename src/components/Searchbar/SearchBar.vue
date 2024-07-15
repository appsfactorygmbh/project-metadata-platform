<template>
  <div style="display: flex; align-items: center; width: 100%">
    <a-input-search
      placeholder="Type what you're looking for:"
      enter-button
      :value="searchStore?.getSearchQuery"
      style="flex-grow: 1; margin-right: 10px"
      @input="onInput"
    />
    <a-button> </a-button>
  </div>
</template>

<script lang="ts" setup>
  import { inject } from 'vue';
  import { type SearchStore } from '@/store/SearchStore';

  const props = defineProps({
    searchStoreSymbol: {
      type: Symbol,
      required: true,
    },
  });

  const searchStore = inject<SearchStore>(props.searchStoreSymbol);

  // Input Listener
  const onInput = (e: Event) => {
    const target = e.target as HTMLInputElement;
    searchStore?.setSearchQuery(target.value);
  };
</script>
