<script lang="ts" setup>
    import SearchableTable from '@/components/Table/SearchableTable.vue';

    const props = defineProps({
    paneWidth: {
      type: Number,
      required: true,
    },
    paneHeight: {
      type: Number,
      required: true,
    },
  });
</script>

<template>
    <SearchableTable :pane-width="props.paneWidth" :pane-height="props.paneHeight" />
</template>