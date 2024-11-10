<script setup lang="ts">
  import { DeleteOutlined } from '@ant-design/icons-vue';
  import { defineProps } from 'vue';
  import { useProjectStore } from '@/store/ProjectsStore.ts';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel.ts';

  const props = defineProps({
    projectId: {
      type: Number,
      required: true,
    },
  });
  const projectStore = useProjectStore();

  const confirmAndArchiveProject = async () => {
    const confirmed = confirm('Are you sure you want to archive this project?');
    if (!confirmed) return;

    await projectStore.archiveProject(props.projectId);
  };

  const archiveButton: FloatButtonModel = {
    name: 'ArchiveButton',
    onClick: confirmAndArchiveProject,
    icon: DeleteOutlined,
    status: 'activated',
    tooltip: 'Click here to archive this project',
  };
</script>

<template>
  <FloatingButton :button="archiveButton" />
</template>

<style scoped></style>
