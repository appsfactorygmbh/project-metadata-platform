<script setup lang="ts">
  import { DeleteOutlined } from '@ant-design/icons-vue';
  import { ref } from 'vue';
  import ConfirmAction from '@/components/Modal/ConfirmAction.vue';
  import { useProjectStore } from '@/store/ProjectsStore.ts';
  import type { FloatButtonModel } from '@/components/Button/FloatButtonModel';

  const projectStore = useProjectStore();
  const isConfirmModalOpen = ref(false);

  const confirmAndArchiveProject = () => {
    isConfirmModalOpen.value = true; // Bestätigungsdialog öffnen
  };

  const handleConfirm = async () => {
    if (projectStore.getProject) {
      const projectData = { ...projectStore.getProject, isArchived: true };
      await projectStore.archiveProject(
        projectData,
        projectStore.getProject.id,
      );
    }
    isConfirmModalOpen.value = false; // Modal schließen nach Bestätigung
  };

  // const confirmAndArchiveProject = async () => {
  //   const confirmed = confirm('Are you sure you want to archive this project?');
  //   if (!confirmed || !projectStore.getProject) return;
  //
  //   const projectData = { ...projectStore.getProject, isArchived: true};
  //
  //   await projectStore.archiveProject(projectData, projectStore.getProject.id);
  // };

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

  <ConfirmAction
    :is-open="isConfirmModalOpen"
    title="Confirm Archive"
    message="Are you sure you want to archive this project?"
    @confirm="handleConfirm"
    @cancel="isConfirmModalOpen = false"
    @update:is-open="(value) => (isConfirmModalOpen = value)"
  />
</template>
