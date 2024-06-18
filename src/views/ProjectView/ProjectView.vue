<script lang="ts" setup>
  import { ProjectView } from '@/components/projectView';

  const props = defineProps({
    paneWidth: {
      type: Number,
      required: true,
    },
  });
</script>

<template>
  <ProjectView :pane-width="props.paneWidth" />
</template>
