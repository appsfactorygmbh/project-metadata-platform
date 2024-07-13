<template>
  <!-- Ant-Design input component -->
  <a-input-search
    placeholder="Type what you're looking for:"
    enter-button
    style="width: 90%"
    @input="onInput"
  />
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
