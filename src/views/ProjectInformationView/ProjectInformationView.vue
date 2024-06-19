<script lang="ts" setup>
  import { ProjectInformation } from '@/components/ProjectInformation';

  const props = defineProps({
    paneWidth: {
      type: Number,
      required: true,
    },
  });
</script>

<template>
  <ProjectInformation :pane-width="props.paneWidth" />
</template>
