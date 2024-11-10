<script setup lang="ts">
  import CreateProjectButton from '@/components/ProjectActionButtons/CreateProjectButton.vue';
  import ArchiveProjectButton from '@/components/ProjectActionButtons/ArchiveProjectButton.vue';
  import { ref } from 'vue';

  const openCreateModal = ref(false);

  const handleOpenUpdate = (status) => {
    console.log('update:open event received with status:', status); // Debugging Log
    openCreateModal.value = status;
  };
</script>

<template>
  <a-float-button-group class="button-group">
    <CreateProjectButton @update:open="handleOpenUpdate" />
    <ArchiveProjectButton />
  </a-float-button-group>

  <!-- Modal für Projekt-Erstellung wird über openCreateModal gesteuert -->
  <slot :open-create-modal="openCreateModal" />
</template>

<style scoped>
  .button-group {
    position: absolute;
    bottom: 20px;
    right: 20px;
  }
</style>
