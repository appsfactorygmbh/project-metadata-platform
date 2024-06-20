<template>
  <!-- Ant-Design input component -->
  <a-input-search
    v-model:value="value"
    placeholder="Type what you're looking for:"
    enter-button
    @input="onInput"
  />
</template>

<script lang="ts" setup>
  import { ref, watch } from 'vue';
  import { searchProjects } from '@/services/SearchService';
  import { debounce } from 'lodash';

  // Variable storing the user-search value
  const value = ref<string>('');

  const searchData = ref<any[]>([]);

  // Asynchronous function to retrieve data from the API
  const fetchData = async () => {
    try {
      const data = await searchProjects(value.value);
      searchData.value = data;
    } catch (error) {
      console.error('Failed to fetch data:', error);
    }
  };

  // Debounced version of fetchData
  const debouncedFetchData = debounce(fetchData, 300);

  // Watcher, which calls fetchData function every time the search value changes
  watch(value, () => {
    debouncedFetchData();
  });

  // Initial data-fetch
  fetchData();

  // Input Listener
  const onInput = (e: Event) => {
    const target = e.target as HTMLInputElement;
    value.value = target.value;
  };
</script>

<style></style>
