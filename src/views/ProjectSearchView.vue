<script setup lang="ts">
  import { ref, reactive } from 'vue';
  import { useElementSize } from '@vueuse/core';
  import Table from '@/components/Table/tableComponent.vue';

  const tablePane = ref(null);
  const dimensions = reactive(useElementSize(tablePane));
</script>

<template>
  <Table :pane-width="dimensions.width" :pane-height="dimensions.height" />
</template>
