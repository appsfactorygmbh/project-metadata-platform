<script setup lang="ts">
  import { DeleteOutlined } from '@ant-design/icons-vue';
  import { useProjectStore } from '@/store/ProjectsStore.ts';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';

  const projectStore = useProjectStore();
  const confirmAndArchiveProject = async () => {
    const confirmed = confirm('Are you sure you want to archive this project?');
    if (!confirmed || !projectStore.getProject?.id) return;
    await projectStore.archiveProject(projectStore.getProject.id);
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
