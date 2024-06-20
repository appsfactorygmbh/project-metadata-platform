<script lang="ts" setup>
  import { ProjectInformation } from '@/components/ProjectInformation';
  import { projectsStoreSymbol } from '@/store/injectionSymbols';
  import { inject, onMounted } from 'vue';

  const props = defineProps({
    paneWidth: {
      type: Number,
      required: true,
    },
    projectId: {
      type: Number,
      required: true,
    },
  });

  const projectStore = inject(projectsStoreSymbol)!;

  onMounted(async () => {
    await projectStore.fetchProject(props.projectId);
  });
</script>

<template>
  <ProjectInformation :pane-width="props.paneWidth" />
</template>
